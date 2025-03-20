using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DatabaseLayer;

/// <summary>
/// Нужен, ТОЛЬКО для миграций т.е. создания фабрики контекста БД для ef migrations
/// dotnet ef migrations add InitialCreate --msbuildprojectextensionspath "..\..\obj\DatabaseLayer"  --project "DatabaseLayer.csproj" --startup-project "..\DatabaseLayer\DatabaseLayer.csproj" --output-dir src\DatabaseLayer\Migrations
/// </summary>
public class DesignTimeCurrencyDbContextFactory: IDesignTimeDbContextFactory<CurrencyDbContext>
{
    public CurrencyDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CurrencyDbContext> options = new DbContextOptionsBuilder<CurrencyDbContext>();
        options.UseNpgsql("Host=localhost;Port=5432;Database=currencyDb;Username=postgres;Password=123;");
        return new CurrencyDbContext(options.Options);
    }
}