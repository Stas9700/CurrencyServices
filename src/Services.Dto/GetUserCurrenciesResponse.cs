using Domain.Models;

namespace CurrencyService.DTO;

public class GetUserCurrenciesResponse
{
    public IReadOnlyCollection<Currency> Currencies { get; init; }
}