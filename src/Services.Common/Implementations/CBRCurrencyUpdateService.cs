using System.Xml;
using System.Xml.Serialization;
using DatabaseLayer;
using DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services.Common.Interfaces;
using Services.Dto.Cbr;

namespace Services.Common.Implementations;

public class CbrCurrencyUpdateService: ICurrencyUpdateService
{
    private readonly IConfiguration _configuration;
    //Используем один и тот же клиент, так как создание нового клиента занимает новый сокет и не освобождает его даже при диспоузе клиента.
    //Да, можно использовать синглтон регистрацию, но зачем если сам сервис синглтон
    private readonly HttpClient _httpClient; 
    private readonly IDbContextFactory<CurrencyDbContext> _dbContextFactory;

    public CbrCurrencyUpdateService(IConfiguration configuration, IDbContextFactory<CurrencyDbContext> dbContextFactory)
    {
        _configuration = configuration;
        _dbContextFactory = dbContextFactory;
        _httpClient = new HttpClient();
    }

    public async Task UpdateCurrenciesAsync()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ValCurs));
        var result = await _httpClient.GetAsync(_configuration.GetConnectionString("Cbr"));
        
        ValCurs currencies;
        using (var stream = result.Content.ReadAsStream())
        {
             currencies = serializer.Deserialize(stream) as ValCurs;
        }
        
        if(currencies == null)
            return;
        
        using (var dbcontext = _dbContextFactory.CreateDbContext())
        {
            var currentCurrencies = dbcontext.Currencies.ToDictionary(d => d.Name);
            foreach (var valute in currencies.Valute)
            {
                if (currentCurrencies.TryGetValue(valute.Name, out var updateValue))
                {
                    updateValue.Rate = double.Parse(valute.VunitRate);
                }
                else
                {
                    dbcontext.Currencies.Add(new Currency()
                    {
                        Name = valute.Name,
                        Rate = double.Parse(valute.VunitRate)
                    });
                }
            }
            
            dbcontext.SaveChanges();
        }
    }
}