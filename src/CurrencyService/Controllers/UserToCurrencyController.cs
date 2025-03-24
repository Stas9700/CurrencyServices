using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Common.Interfaces;
using Services.Dto;

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
    
    [Authorize]
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
    
    [Authorize]
    [HttpGet("getCurrencies")]
    public async Task<GetUserCurrenciesResponse> GetCurrencies()
    {
        var currencies = await _userCurrenciesService.GetCurrenciesAsync();
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
    
    [Authorize]
    [HttpPost("setUserCurrency")]
    public async Task SetUserCurrency(SetUserCurrencyRequest request)
    {
        await _userCurrenciesService.SetUserCurrencyAsync(request.UserId, request.CurrencyId);
    }
}