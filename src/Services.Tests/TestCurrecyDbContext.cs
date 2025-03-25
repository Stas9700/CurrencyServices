using DatabaseLayer;
using Microsoft.EntityFrameworkCore;

namespace Services.Tests;

public class TestCurrencyDbContext : CurrencyDbContext
{
    public TestCurrencyDbContext(DbContextOptions<CurrencyDbContext> options) 
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Используем InMemory database для тестов
        optionsBuilder.UseInMemoryDatabase("TestDatabase");
    }
}