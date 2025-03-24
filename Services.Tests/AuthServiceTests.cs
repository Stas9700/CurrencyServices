using DatabaseLayer;
using DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services.Common.Implementations;

namespace Services.Tests;

public class AuthServiceTests
{
    private readonly IDbContextFactory<CurrencyDbContext> _dbContextFactory;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<CurrencyDbContext>()
            .UseInMemoryDatabase(databaseName: "AuthServiceTests")
            .Options;
        
        _dbContextFactory = new TestDbContextFactory<CurrencyDbContext>(options);
        _authService = new AuthService(_dbContextFactory);
    }

    [Fact]
    public async Task AuthenticateUser_UserExistsAndPasswordMatches_ReturnsTrue()
    {
        // Arrange
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            context.Users.Add(new User { Username = "testuser", Password = "password123" });
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _authService.AuthenticateUser("testuser", "password123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AuthenticateUser_UserDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _authService.AuthenticateUser("nonexistent", "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RegisterUser_NewUser_ReturnsTrueAndAddsUser()
    {
        // Act
        var result = await _authService.RegisterUser("newuser", "password");

        // Assert
        Assert.True(result);
        
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            Assert.True(await context.Users.AnyAsync(u => u.Username == "newuser"));
        }
    }

    [Fact]
    public async Task RegisterUser_UserExists_ReturnsFalse()
    {
        // Arrange
        using (var context = await _dbContextFactory.CreateDbContextAsync())
        {
            context.Users.Add(new User { Username = "existing", Password = "pass" });
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _authService.RegisterUser("existing", "newpass");

        // Assert
        Assert.False(result);
    }
}