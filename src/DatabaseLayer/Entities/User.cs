namespace DatabaseLayer.Entities;

public class User
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Password { get; set; }
    
    public IEnumerable<UserToCurrency>? UserToCurrencies { get; init; }
}