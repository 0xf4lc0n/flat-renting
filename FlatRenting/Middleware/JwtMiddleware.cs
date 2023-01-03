using FlatRenting.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlatRenting.Middleware;

public class JwtMiddleware {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public JwtMiddleware(RequestDelegate next, IConfiguration config, ILogger logger) {
        _next = next;
        _config = config;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx, IUserRepository userRepository) {
        var token = ExtractBearerdToken(ctx);

        if (token != null) {
            await SaveUserInCtx(ctx, userRepository, token);
        }

        await _next(ctx);
    }

    private async Task SaveUserInCtx(HttpContext ctx, IUserRepository userRepository, string token) {
        try {
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

            var user = await userRepository.GetUser(userId);
            ctx.Items["User"] = user;
        } catch (Exception ex) {
            _logger.Error(ex, "Cannot save user in HttpContext");
        }

    }

    private static string? ExtractBearerdToken(HttpContext ctx) => ctx.Request.Headers.Authorization.FirstOrDefault()?.Split(' ').Last();
}
