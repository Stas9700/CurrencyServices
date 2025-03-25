using DatabaseLayer;
using DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services.Common.Implementations;

namespace Services.Tests;

public class UserCurrenciesServiceTests
{
    private readonly IDbContextFactory<CurrencyDbContext> _dbContextFactory;
    private readonly UserCurrenciesService _service;

    public UserCurrenciesServiceTests()
    {
        var options = new DbContextOptionsBuilder<CurrencyDbContext>()
            .UseInMemoryDatabase(databaseName: "UserCurrenciesServiceTests")
            .Options;
        
        _dbContextFactory = new TestDbContextFactory<CurrencyDbContext>(options);
        _service = new UserCurrenciesService(_dbContextFactory);
    }

    [Fact]
    public async Task GetUserCurrenciesAsync_UserExists_ReturnsCurrencies()
    {
        // Arrange
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var currency = new Currency { Name = "USD", Rate = 1.0 };
            var user = new User 
            { 
                Username = "testuser",
                Password = "pass",
                UserToCurrencies = new List<UserToCurrency>
                {
                    new UserToCurrency { Currency = currency }
                }
            };
            
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        var userId = 1; // InMemory database обычно начинает ID с 1
        var result = await _service.GetUserCurrenciesAsync(userId);

        // Assert
        Assert.Single(result);
        Assert.Equal("USD", result.First().Name);
    }

    [Fact]
    public async Task SetUserCurrencyAsync_NewCurrency_AddsCurrency()
    {
        // Arrange
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            context.Users.Add(new User { Username = "testuser", Password = "pass" });
            context.Currencies.Add(new Currency { Name = "EUR", Rate = 0.85 });
            await context.SaveChangesAsync();
        }

        // Act
        var userId = 1;
        var currencyId = 1;
        await _service.SetUserCurrencyAsync(userId, currencyId);

        // Assert
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            var userCurrency = await context.UserToCurrencies
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CurrencyId == currencyId);
            
            Assert.NotNull(userCurrency);
        }
    }
}