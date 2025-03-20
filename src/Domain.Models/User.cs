namespace Domain.Models;

public class User
{
    public int Id { get; init; }
    public string Login { get; init; }
    
    public IEnumerable<Currency> FavoriteCurrencies { get; init; }
}