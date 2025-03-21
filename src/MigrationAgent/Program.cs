// See https://aka.ms/new-console-template for more information


using DatabaseLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString(nameof(CurrencyDbContext));
        Console.WriteLine(connectionString);
        
        IServiceCollection services = new ServiceCollection();
        services.AddDbContextFactory<CurrencyDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString);
        });

        var dbContextFactory = services.BuildServiceProvider().GetService<IDbContextFactory<CurrencyDbContext>>();

        try
        {
            using (CurrencyDbContext dbContext = await dbContextFactory?.CreateDbContextAsync()!)
            {
                Console.WriteLine("Уже накатываем миграции.");
                var t = await dbContext?.Database.GetAppliedMigrationsAsync()!;
                await dbContext?.Database.MigrateAsync()!;
            }
            Console.WriteLine("База данных обновлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private static string? GetConfigDirectoryPath(bool designMode)
    {
        var runtimeConfigPath = Directory.GetCurrentDirectory();
        var designConfigPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        var path = designMode ? designConfigPath : runtimeConfigPath;
        return path;
    }
    
    public static string GetDatabaseFileLocation()
    {
        const string dbName = "cache.db";

        var currentDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        var fullPath = Path.Combine(currentDirectoryPath, dbName);
        return fullPath;
    }
}