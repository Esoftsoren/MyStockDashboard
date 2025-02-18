using CurrencyConverterCustom;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using MyStockDashboard.Data;
using MyStockDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();       
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
// Register the HttpClient for the converter.
builder.Services.AddHttpClient("CurrencyConverterClient", client =>
{
    client.BaseAddress = new Uri("https://api.exchangerate.host/");
});
builder.Services.AddSingleton<CurrencyConverterUrlOptions>(sp =>
    new CurrencyConverterUrlOptions("https://api.exchangerate.host")
    {
        BaseSymbol = "EUR",  
        AccessKey = "6819903fe623f480d59622653ee4dc49"
    });

// Register your custom converter.
builder.Services.AddSingleton<Converter>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient("CurrencyConverterClient");
    var options = sp.GetRequiredService<CurrencyConverterUrlOptions>();
    return new Converter(client, options);
});

// Register your conversion service.
builder.Services.AddSingleton<ICurrencyConversionService, RealCurrencyConversionService>();
builder.Services.AddHostedService<PressReleaseScraperService>();
builder.Services.AddHostedService<StockHistoryUpdaterService>();
builder.Services.AddScoped<StockStateService>();
builder.Services.AddHttpClient<StockDataService>(); 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<StockDataService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Map the Blazor Hub.
app.MapBlazorHub();

// Map fallback requests to the _Host page.
app.MapFallbackToPage("/_Host");

app.Run();