using System.Text.Json;
using System.Web;
using MyStockDashboard.Models;
using YahooFinanceApi;

namespace MyStockDashboard.Services;

public class StockDataService
{
    private readonly HttpClient _httpClient;
    private DateTime _lastSearchTime = DateTime.MinValue;
    private readonly TimeSpan _debounceTime = TimeSpan.FromMilliseconds(500);
    private readonly string _alphaVantageApiKey = "LRY9V1KWR2DLKL15";

    public StockDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Security> GetStockDataAsync(string symbol)
    {
        var securities = await Yahoo.Symbols(symbol)
            .Fields(
                Field.Symbol,
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
                return Enumerable.Empty<Stock>();

            // Debounce: only send a request if enough time has passed.
            if (DateTime.Now - _lastSearchTime < _debounceTime)
                return Enumerable.Empty<Stock>();

            _lastSearchTime = DateTime.Now;

            // URL‑encode the query.
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={encodedQuery}&apikey={_alphaVantageApiKey}";

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
}
