namespace DatabaseLayer.Entities;

public class UserToCurrency
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int CurrencyId { get; init; }
    
    public User User { get; init; }
    public Currency Currency { get; init; }
}