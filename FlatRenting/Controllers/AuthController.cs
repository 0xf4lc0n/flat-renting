using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
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
    private readonly ILogger _logger;
    private readonly IConfiguration _config;

    public AuthController(IUserRepository userRepository, ILogger logger, IConfiguration config) {
        _userRepository = userRepository;
        _logger = logger;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto) {
        try {
            await _userRepository.AddUser(registerDto);
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot register user with data {@registerDto}", registerDto);
            return BadRequest("Given email address or login are taken");
        }

        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto) {

        try {
            var user = await _userRepository.GetUser(loginDto.Login, loginDto.Password);

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
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot login user with data {@loginDto}", loginDto);
            return BadRequest("Login or password are incorrect");
        }

        return Ok();
    }

    [HttpGet("activate")]
    public void Activate() {
        Ok();
    }
}
