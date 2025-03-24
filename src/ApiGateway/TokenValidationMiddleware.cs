using Services.Common.Interfaces;

namespace ApiGateway;


public class TokenValidationMiddleware(RequestDelegate next, ITokenBlacklistService tokenBlacklist)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Извлекаем токен из заголовка Authorization
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token) && !tokenBlacklist.IsTokenValid(token))
        {
            // Если токен в черном списке, возвращаем 401 Unauthorized
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token is invalid or revoked.");
            return;
        }

        // Передаем запрос дальше по конвейеру
        await next(context);
    }
}