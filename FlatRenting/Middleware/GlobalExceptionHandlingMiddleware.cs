using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FlatRenting.Middleware;

public class GlobalExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx) {
        try {
            await _next(ctx);
        } catch (Exception ex) {
            _logger.Error(ex, ex.Message);
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/json";

            var problem = new ProblemDetails {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server error",
                Title = "Server error",
                Detail = "An internal server error has occurred"
            };

            var json = JsonSerializer.Serialize(problem);
            await ctx.Response.WriteAsync(json);
        }
    }
}
