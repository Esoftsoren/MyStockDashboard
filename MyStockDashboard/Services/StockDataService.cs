using System.Text.Json;
using System.Web;
using Microsoft.EntityFrameworkCore;
using MyStockDashboard.Data;
using MyStockDashboard.Models;
using YahooFinanceApi;

namespace MyStockDashboard.Services;

public class StockDataService
{
    private readonly HttpClient _httpClient;
    private DateTime _lastSearchTime = DateTime.MinValue;
    private readonly TimeSpan _debounceTime = TimeSpan.FromMilliseconds(500);
    private readonly string _alphaVantageApiKey = "LRY9V1KWR2DLKL15";
    private string _lastQuery = string.Empty;
    private readonly IServiceProvider _serviceProvider;

    public StockDataService(HttpClient httpClient, IServiceProvider serviceProvider)
    {
        _httpClient = httpClient;
        _serviceProvider = serviceProvider;
    }

    public async Task<Security> GetStockDataAsync(string symbol)
    {
        var securities = await Yahoo.Symbols(symbol)
            .Fields(
                Field.Symbol,
                Field.Currency,
                Field.RegularMarketPrice,
                Field.RegularMarketChange,
                Field.RegularMarketChangePercent,
                Field.RegularMarketDayHigh,
                Field.RegularMarketDayLow,
                Field.LongName
            )
            .QueryAsync();

        if (securities.TryGetValue(symbol, out var security))
        {
            return security;
        }

        return null;
    }

    public async Task<IEnumerable<Stock>> SearchStocksAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            _lastQuery = "";
            return Enumerable.Empty<Stock>();
        }

        // Set the current query as the last query.
        _lastQuery = query;

        // Wait for the debounce interval.
        await Task.Delay(_debounceTime);

        // If the query has changed during the wait, cancel this search.
        if (query != _lastQuery)
            return Enumerable.Empty<Stock>();

        // URL‑encode the query.
        var encodedQuery = Uri.EscapeDataString(query);
        var url =
            $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={encodedQuery}&apikey={_alphaVantageApiKey}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            // Optionally log the error.
            return Enumerable.Empty<Stock>();
        }

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var list = new List<Stock>();

        // Alpha Vantage returns a JSON object with a "bestMatches" property.
        if (root.TryGetProperty("bestMatches", out var matches))
        {
            foreach (var match in matches.EnumerateArray())
            {
                // Keys are returned as "1. symbol" and "2. name"
                if (match.TryGetProperty("1. symbol", out var symbolProp) &&
                    match.TryGetProperty("2. name", out var nameProp))
                {
                    var symbol = symbolProp.GetString();
                    var name = nameProp.GetString();
                    if (!string.IsNullOrWhiteSpace(symbol))
                    {
                        list.Add(new Stock
                        {
                            Symbol = symbol,
                            CompanyName = name,
                        });
                    }
                }
            }
        }

        return list;
    }

    public async Task RemoveStockFromUserPortfolioAsync(string symbol)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var portfolioStocks = await dbContext.UserPortfolioStocks
            .Where(x => x.Symbol == symbol)
            .ToListAsync();
        dbContext.UserPortfolioStocks.RemoveRange(portfolioStocks);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddStockToUserPortfolioAsync(Stock stock)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
        // Log: Check if the stock already exists.
        if (await dbContext.UserPortfolioStocks.AnyAsync(x => x.Symbol == stock.Symbol))
        {
            Console.WriteLine($"Portfolio already contains stock: {stock.Symbol}");
            return;
        }
    
        // Create and add the portfolio record.
        var portfolioStock = new UserPortfolioStock
        {
            Symbol = stock.Symbol,
            CompanyName = stock.CompanyName
        };
        dbContext.UserPortfolioStocks.Add(portfolioStock);
        var result = await dbContext.SaveChangesAsync();
        Console.WriteLine($"Added {stock.Symbol} to portfolio. Rows affected: {result}");
    }
    
    public async Task<List<string>> GetUserPortfolioSymbolsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.UserPortfolioStocks
            .Select(x => x.Symbol)
            .ToListAsync();
    }
    public async Task InsertStockPriceHistoryAsync(string symbol, double price)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var history = new StockPriceHistory
            {
                Id = Guid.NewGuid(),
                Symbol = symbol,
                Timestamp = DateTime.Now,
                Price = (decimal)price // Convert to decimal for the database field
            };
            dbContext.StockPriceHistories.Add(history);
            await dbContext.SaveChangesAsync();
        }
    }
}

