//using CurrencyGrpsService.Services;

using CurrencyGrpsService.Services;
using Services.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Build(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserCurrenciesGrpcService>();

app.Run();