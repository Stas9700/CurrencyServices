using DatabaseLayer;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService.DI;

public static class ApplicationDiBuilder
{
    private static IConfiguration _configuration;
    public static IServiceCollection Build(this IServiceCollection services,IConfiguration configuration)
    {
        _configuration = configuration;
        services.AddDbContextFactory<CurrencyDbContext>(CreateDbContext);
        return services;
    }
    
    private static void CreateDbContext(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(_configuration.GetConnectionString(nameof(CurrencyDbContext)));
    }
}