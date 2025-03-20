using CurrencyService.DTO;
using DatabaseLayer;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService.Controlers;

[ApiController]
[Route("api/currency")]
public class UserToCurrencyController: ControllerBase
{
    private readonly IDbContextFactory<CurrencyDbContext> _dbContextFactory;

    public UserToCurrencyController(IDbContextFactory<CurrencyDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [HttpPost("getUserCurrencies")]
    public async Task<GetUserCurrenciesResponse> GetUserCurrencies(GetUserCurrenciesRequest request)
    {
        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var userInfo = dbContext.Users
                .Include(i => i.UserToCurrencies)
                .ThenInclude(t => t.Currency)
                .FirstOrDefault(w => w.Id == request.UserId);

            return new GetUserCurrenciesResponse()
            {
                Currencies = userInfo?.UserToCurrencies.Select(s => new Currency()
                {
                    Id = s.Currency.Id,
                    Name = s.Currency.Name,
                    Rate = s.Currency.Rate,
                }).ToArray()
            };
        }
    }
}