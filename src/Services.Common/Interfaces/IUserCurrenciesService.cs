using Domain.Models;

namespace Services.Common.Interfaces;

public interface IUserCurrenciesService
{
    public Task<IReadOnlyCollection<Currency>> GetUserCurrenciesAsync(int userId);
}