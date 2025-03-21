using DatabaseLayer;
using DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Common.Interfaces;

namespace Services.Common.Implementations;

public class AuthService: IAuthService
{
    private readonly IDbContextFactory<CurrencyDbContext> _dbContextFactory;

    public AuthService(IDbContextFactory<CurrencyDbContext> context)
    {
        _dbContextFactory = context;
    }

    public async Task<bool> AuthenticateUser(string username, string password)
    {
        User user;
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            user = dbContext.Users.SingleOrDefault(u => u.Username == username);
        }

        if (user == null)
        {
            // Пользователь не найден
            return false;
        }

        // Проверяем пароль.
        return user.Password == password;

    }

    public async Task<bool> RegisterUser(string username, string password)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var user = dbContext.Users.SingleOrDefault(u => u.Username == username);
            if (user != null)
                return false;

            dbContext.Users.Add(new User()
            {
                Username = username,
                Password = password
            });

            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}