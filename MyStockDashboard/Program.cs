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
builder.Services.AddHostedService<PressReleaseScraperService>();
builder.Services.AddHostedService<StockHistoryUpdaterService>();
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