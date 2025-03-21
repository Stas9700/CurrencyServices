using CurrencyService.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Common.Interfaces;

namespace CurrencyService.Controlers;

[ApiController]
[Route("api/currency")]
public class UserToCurrencyController : ControllerBase
{
    private readonly IUserCurrenciesService _userCurrenciesService;

    public UserToCurrencyController(IUserCurrenciesService userCurrenciesService)
    {
        _userCurrenciesService = userCurrenciesService;
    }
    
    //[Authorize]
    [HttpPost("getUserCurrencies")]
    public async Task<GetUserCurrenciesResponse> GetUserCurrencies(GetUserCurrenciesRequest request)
    {
        var currencies = await _userCurrenciesService.GetUserCurrenciesAsync(request.UserId);
        return new GetUserCurrenciesResponse()
        {
            Currencies = currencies.Select(s => new Currency()
            {
                Id = s.Id,
                Name = s.Name,
                Rate = s.Rate,
            }).ToArray()
        };
    }
}