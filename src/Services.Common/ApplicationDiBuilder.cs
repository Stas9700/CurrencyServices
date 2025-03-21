using DatabaseLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Common.Implementations;
using Services.Common.Interfaces;

namespace Services.Common;

public static class ApplicationDiBuilder
{
    private static IConfiguration _configuration;
    public static IServiceCollection Build(this IServiceCollection services,IConfiguration configuration)
    {
        _configuration = configuration;
        services.AddSingleton(_configuration)
                .AddDbContextFactory<CurrencyDbContext>(CreateDbContext)
                .AddServices();
        return services;
    }
    
    private static void CreateDbContext(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(_configuration.GetConnectionString(nameof(CurrencyDbContext)));
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserCurrenciesService, UserCurrenciesService>();
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
        
        return services;
    }
}