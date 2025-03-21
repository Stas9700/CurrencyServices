using DatabaseLayer;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Services.Common.Interfaces;

namespace Services.Common.Implementations;

public class UserCurrenciesService: IUserCurrenciesService
{
    private readonly IDbContextFactory<CurrencyDbContext> _dbContextFactory;

    public UserCurrenciesService(IDbContextFactory<CurrencyDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyCollection<Currency>> GetUserCurrenciesAsync(int userId)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var userInfo = dbContext.Users
                .Include(i => i.UserToCurrencies)
                .ThenInclude(t => t.Currency)
                .FirstOrDefault(w => w.Id == userId);

            return userInfo?.UserToCurrencies.Select(s => new Currency()
            {
                Id = s.Currency.Id,
                Name = s.Currency.Name,
                //Rate = s.Currency.Rate,
            }).ToArray();
        }
    }
}