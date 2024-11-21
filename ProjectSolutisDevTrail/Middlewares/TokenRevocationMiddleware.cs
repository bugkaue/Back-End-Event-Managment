using ProjectSolutisDevTrail.Services.Implementations;

namespace ProjectSolutisDevTrail.Middlewares;

public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenRevocationService _tokenRevocationService;

    public TokenRevocationMiddleware(RequestDelegate next, TokenRevocationService tokenRevocationService)
    {
        _next = next;
        _tokenRevocationService = tokenRevocationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (_tokenRevocationService.IsTokenRevoked(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}