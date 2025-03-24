using Domain.Models;

namespace Services.Dto;

public class GetUserCurrenciesResponse
{
    public IReadOnlyCollection<Currency> Currencies { get; init; }
}