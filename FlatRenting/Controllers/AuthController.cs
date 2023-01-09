using FlatRenting.Data;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using FlatRenting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

    public AuthController(IUserRepository userRepository, ILogger logger, IConfiguration config, IEmailService email, IActivationRepository activationRepository) {
        _userRepository = userRepository;
        _logger = logger;
        _config = config;
        _email = email;
        _activationRepository = activationRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto) {
        Guid userId;

        try {
            userId = await _userRepository.AddUser(registerDto);
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
            var activationUrl = $"{_config["Kestrel:Endpoints:HttpsEndpoint:Url"]}/api/auth/activate/{activationCode}";
            var message = $"Aby aktywować konto kliknij w link: {activationUrl}";
            var emailContent = new EmailData("Aktywacja konta w serwisie Flat Lender", message, message);
            await _email.SendEmail(receiverData, emailContent);
        } catch (EmailException ex) {
            _logger.Error(ex, "Cannot send activation code to the user with data {@registerDto}", registerDto);
            return BadRequest("Fatal error. Please contact administration");
        }

        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto) {

        try {
            var user = await _userRepository.GetUser(loginDto.Login, loginDto.Password);

            if (!user.IsActive) {
                return BadRequest("Account have to be activated. Check your email address.");
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
}
