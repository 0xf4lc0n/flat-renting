using FlatRenting.Data;
using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using FlatRenting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlatRenting.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase {
    private readonly IUserRepository _userRepository;
    private readonly IActivationRepository _activationRepository;
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly IEmailService _email;
    private readonly ICryptoService _crypto;
    private readonly IRandomService _random;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public AuthController(IUserRepository userRepository, ILogger logger, IConfiguration config, IEmailService email, IActivationRepository activationRepository, ICryptoService crypto, IRandomService random, IConnectionMultiplexer redis) {
        _userRepository = userRepository;
        _logger = logger;
        _config = config;
        _email = email;
        _activationRepository = activationRepository;
        _crypto = crypto;
        _random = random;
        _redis = redis;
        _database = _redis.GetDatabase();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto) {
        Guid userId;

        try {
            var dto = _crypto.HashPassword(registerDto);
            userId = await _userRepository.AddUser(dto);
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot register user with data {@registerDto}", registerDto);
            return BadRequest("Given email address or login are taken");
        }


        Guid activationCode;

        try {
            activationCode = await _activationRepository.AddActivation(userId);
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot generate activation code for user with data {@registerDto}", registerDto);
            return BadRequest("Fatal error. Please contact administration");
        }

        try {
            var receiverData = new ReceiverData(registerDto.Email, $"{registerDto.FirstName} {registerDto.LastName}");
            var activationUrl = $"{_config["ConfirmationBaseUrl"]}/api/auth/activate/{activationCode}";
            var message = $"Aby aktywować konto kliknij w link: {activationUrl}";
            var emailContent = new EmailData("Aktywacja konta w serwisie Flat Lender", message, message);
            _email.SendEmail(receiverData, emailContent);
        } catch (EmailException ex) {
            _logger.Error(ex, "Cannot send activation code to the user with data {@registerDto}", registerDto);
            return BadRequest("Fatal error. Please contact administration");
        }

        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto) {

        try {
            var user = await _userRepository.GetUserByLogin(loginDto.Login);

            if (!user.IsActive) {
                return BadRequest("Account have to be activated. Check your email address.");
            }

            if (!_crypto.IsPasswordValid(loginDto.Password, user.Password)) {
                return BadRequest("Login or password are incorrect");
            }

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateEncodedJwt(tokenDescriptor);
            _logger.Information("Token: {Token}", token);

            HttpContext.Response.Headers.Add("Authorization", "Bearer " + token);

            return Ok(user.ToLoggedDto());
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot login user with data {@loginDto}", loginDto);
            return BadRequest("Login or password are incorrect");
        }
    }

    [HttpGet("activate/{activation_code}")]
    public async Task<IActionResult> Activate(Guid activation_code) {
        try {
            var activation = await _activationRepository.GetActivation(activation_code);

            await _activationRepository.DeleteActivation(activation);

            if (activation.ValidTo < DateTime.UtcNow) {
                await _userRepository.DeleteUser(activation.UserId);
                return BadRequest("Activation code has expired");
            }

            await _userRepository.ActivateUser(activation.UserId);
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot activate user account");
            return BadRequest("Cannot activate account");
        }

        return Ok();
    }

    [HttpGet("password/forget")]
    public async Task<IActionResult> Forget(string emailAddress) {
        User user;

        try {
            user = await _userRepository.GetUser(emailAddress);
        } catch (RepositoryException ex) {
            _logger.Error(ex, $"Cannot reset password for user with email address '{emailAddress}'");
            return Ok();
        }

        var code = _random.GenerateRandomString(8);
        _database.StringSet(emailAddress, code);
        _database.KeyExpire(emailAddress, DateTime.Now.AddMinutes(5));

        var receiverData = new ReceiverData(user.Email, $"{user.FirstName} {user.LastName}");
        var message = $"Aby zresetować hasło użyj następującego kodu: {code}";
        var emailContent = new EmailData("Resetowanie hasła w serwisie Flat Lender", message, message);
        _email.SendEmail(receiverData, emailContent);

        return Ok();
    }

    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto) {
        var code = _database.StringGet(resetPasswordDto.Email);

        if (code.IsNull) {
            _logger.Error("Reset code for user '{Email}' is not present", resetPasswordDto.Email);
            return BadRequest("Cannot reset password");
        }

        if (code != resetPasswordDto.Code) {
            _logger.Error("User '{Email}' provided invalid code for password reset", resetPasswordDto.Email);
            return BadRequest("Cannot reset password");
        }

        var newPassword = _crypto.HashPassword(resetPasswordDto.NewPassword);

        try {
            await _userRepository.ChangeUserPassword(resetPasswordDto.Email, newPassword);
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot reset password for user '{Email}'", resetPasswordDto.Email);
            return BadRequest("Cannot reset password");
        }

        return Ok();
    }

}
