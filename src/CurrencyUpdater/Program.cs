using System.Text;
using CurrencyUpdater;
using DatabaseLayer;
using Microsoft.EntityFrameworkCore;
using Services.Common.Implementations;
using Services.Common.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContextFactory<CurrencyDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CurrencyDbContext"));
});
builder.Services.AddSingleton<ICurrencyUpdateService, CbrCurrencyUpdateService>();
builder.Services.AddHostedService<Updater>();

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var host = builder.Build();
host.Run();