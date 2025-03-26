using CurrencyService.Grpc;
using Grpc.Core;
using Services.Common.Interfaces;

namespace CurrencyGrpsService.Services;

public class UserCurrenciesGrpcService: UserCurrenciesService.UserCurrenciesServiceBase
{
    private readonly IUserCurrenciesService _userCurrenciesService;

    public UserCurrenciesGrpcService(IUserCurrenciesService userCurrenciesService)
    {
        _userCurrenciesService = userCurrenciesService;
    }
    
    public override async Task<GetUserCurrenciesResponse> GetUserCurrencies(
        GetUserCurrenciesRequest request, 
        ServerCallContext serverCallContext)
    {
        var currencies = await _userCurrenciesService.GetUserCurrenciesAsync(request.UserId);
            
        return new GetUserCurrenciesResponse
        {
            Currencies = { currencies.Select(ToGrpcCurrency) }
        };
    }

    public override async Task<GetCurrenciesResponse> GetCurrencies(
        GetCurrenciesRequest request, 
        ServerCallContext context)
    {
        var currencies = await _userCurrenciesService.GetCurrenciesAsync();
            
        return new GetCurrenciesResponse
        {
            Currencies = { currencies.Select(ToGrpcCurrency) }
        };
    }

    public override async Task<SetUserCurrencyResponse> SetUserCurrency(
        SetUserCurrencyRequest request, 
        ServerCallContext context)
    {
        await _userCurrenciesService.SetUserCurrencyAsync(request.UserId, request.CurrencyId);
            
        return new SetUserCurrencyResponse
        {
            Success = true
        };
    }
    
    private static Currency ToGrpcCurrency(Domain.Models.Currency currency)
    {
        return new Currency
        {
            Id = currency.Id,
            Name = currency.Name,
            Rate = currency.Rate
        };
    }
}