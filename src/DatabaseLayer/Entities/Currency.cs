namespace DatabaseLayer.Entities;

public class Currency
{
    public int Id { get; init; }
    public string Name { get; set; }
    public double Rate { get; set; }
    
    public IEnumerable<UserToCurrency>? UserToCurrencies { get; init; }
}