using DatabaseLayer;
using Microsoft.EntityFrameworkCore;
using Services.Common.Implementations;
using Services.Common.Interfaces;

namespace CurrencyService.DI;

public static class ApplicationDiBuilder
{
    private static IConfiguration _configuration;
    public static IServiceCollection Build(this IServiceCollection services,IConfiguration configuration)
    {
        _configuration = configuration;
        services.AddDbContextFactory<CurrencyDbContext>(CreateDbContext)
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

        return services;
    }
}