using Services.Common.Interfaces;

namespace CurrencyUpdater;

public class Updater : BackgroundService
{
    
    private readonly ICurrencyUpdateService _currencyUpdateService;
    
    public Updater(ICurrencyUpdateService currencyUpdateService)
    {
        _currencyUpdateService = currencyUpdateService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _currencyUpdateService.UpdateCurrenciesAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}