using Microsoft.EntityFrameworkCore;
using MyStockDashboard.Data;
using MyStockDashboard.Models;

namespace MyStockDashboard.Services;

public class StockHistoryUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public StockHistoryUpdaterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // This loop runs until the application is shutting down.
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Get the list of stocks to update history for.
                var stocks = await dbContext.Stocks.ToListAsync(stoppingToken);
                foreach (var stock in stocks)
                {
                    // Create a new history record. (You might want to adjust how you get StockPrice)
                    var history = new StockPriceHistory
                    {
                        Symbol = stock.Symbol,
                        Timestamp = DateTime.Now,
                        Price = stock.StockPrice
                    };
                    dbContext.StockPriceHistories.Add(history);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
    
    