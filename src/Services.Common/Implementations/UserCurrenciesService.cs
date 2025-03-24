using DatabaseLayer;
using DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Common.Interfaces;
using Currency = Domain.Models.Currency;

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
            ///Если при большой нагрузке запрос будет работать долго,
            /// можно переписать его через join-ы. Так запрос не будет содержать подзапросов.
            var userInfo = dbContext.Users
                .Include(i => i.UserToCurrencies)!
                .ThenInclude(t => t.Currency)
                .FirstOrDefault(w => w.Id == userId);

            return userInfo?.UserToCurrencies.Select(s => new Currency()
            {
                Id = s.Currency.Id,
                Name = s.Currency.Name,
                Rate = s.Currency.Rate,
            }).ToArray();
        }
    }

    public async Task<IReadOnlyCollection<Currency>> GetCurrenciesAsync()
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var currencies = dbContext.Currencies.ToArray();

            return currencies.Select(s => new Currency()
            {
                Id = s.Id,
                Name = s.Name,
                Rate = s.Rate,
            }).ToArray();
        }
    }

    public async Task SetUserCurrencyAsync(int userId, int currencyId)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var current = dbContext.UserToCurrencies
                .SingleOrDefault(w => w.CurrencyId == currencyId 
                            && w.UserId == userId);


            if (current != null) 
                return;

            dbContext.UserToCurrencies.Add(new UserToCurrency()
            {
                CurrencyId = currencyId,
                UserId = userId,
            });

            dbContext.SaveChanges();
        }
    }
}