using Microsoft.EntityFrameworkCore;

namespace Services.Tests;

// Вспомогательный класс для создания DbContextFactory
public class TestDbContextFactory<TContext> : IDbContextFactory<TContext> 
    where TContext : DbContext
{
    private readonly DbContextOptions<TContext> _options;

    public TestDbContextFactory(DbContextOptions<TContext> options)
    {
        _options = options;
    }

    public TContext CreateDbContext()
    {
        return (TContext)System.Activator.CreateInstance(typeof(TContext), _options);
    }

    public async Task<TContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(CreateDbContext());
    }
}