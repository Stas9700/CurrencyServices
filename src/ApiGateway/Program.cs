using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApiGateway;
using CurrencyService.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.Common;
using Services.Common.Interfaces;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.Build(builder.Configuration);

        var key = builder.Configuration.GetValue<string>("JwtKey");

        if (key.IsNullOrEmpty())
            throw new NotImplementedException("JWT token key not found");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidAudience = AuthOptions.AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });


        builder.Services.AddAuthorization();

// Регистрация HttpClient для каждого микросервиса
        builder.Services.AddHttpClient("UserAuthentication",
            client => { client.BaseAddress = new Uri("http://localhost:5058"); });

        builder.Services.AddHttpClient("CurrencyService",
            client => { client.BaseAddress = new Uri("http://localhost:5000"); });

        builder.Services.AddGrpcClient<UserCurrenciesService.UserCurrenciesServiceClient>(options =>
        {
            options.Address = new Uri("http://localhost:5002");
        });


        var app = builder.Build();

        app.UseRouting();

        app.UseMiddleware<TokenValidationMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

// Маршрутизация запросов
        app.Map("api/UserAuthentication/logout",
                async (HttpContext context, ITokenBlacklistService tokenBlacklistService) =>
                {
                    // Извлекаем токен из заголовка Authorization
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    // Добавляем токен в черный список
                    tokenBlacklistService.AddToken(token, jwtToken.ValidTo);

                    context.Response.StatusCode = StatusCodes.Status200OK;
                })
            .RequireAuthorization();


        app.Map("api/UserAuthentication/{**path}",
            async (HttpContext context, IHttpClientFactory httpClientFactory) =>
            {
                await RequestForwardExtensions.ForwardRequest(context, httpClientFactory, "UserAuthentication");
            });

        app.Map("api/currency/{**path}",
                async (HttpContext context, IHttpClientFactory httpClientFactory) =>
                {
                    await RequestForwardExtensions.ForwardRequest(context, httpClientFactory, "CurrencyService");
                })
            .RequireAuthorization();

        app.Map("/httptogrpc/usercurrencies/{**method}",
            async (HttpContext context, UserCurrenciesService.UserCurrenciesServiceClient client) =>
            {
                try
                {
                    var method = context.Request.RouteValues["method"]?.ToString()?.ToLower();
                    await GrpcExtensions.GrpsRequestHandler(method, context, client);
                }
                catch (RpcException ex)
                {
                    context.Response.StatusCode = ex.StatusCode.MapGrpcStatusCodeToHttp();
                    await context.Response.WriteAsync($"gRPC error: {ex.Status.Detail}");
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"Error processing gRPC request: {ex.Message}");
                }
            })
            .RequireAuthorization();
        
        /// Так же можно было бы обрабатывать не http запросы и перенапрявлять из в grpc сервис,
        /// а использовать пакет Yarp.ReverseProxy.
        /// В таком случае мы бы просто перенаправляли grps request на нужный сервис.
        /// Однако проверку токена пришлось бы реализовать на уровне TokenValidationMiddleware
        /// И мы проверяли бы не только отсутствие токена в черном списке, но и впринципе его валидность. 

        app.Run();
    }

    
}