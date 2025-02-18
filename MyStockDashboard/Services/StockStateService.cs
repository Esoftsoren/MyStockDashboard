using ChartJs.Blazor.LineChart;

namespace MyStockDashboard.Services;

public class StockStateService
{
    public string CurrentSymbol { get; set; }
    public YahooFinanceApi.Security DetailedSecurity { get; set; }
    public List<(DateTime Time, double Price, string Currency)>? HistoricalData { get; set; } = new();
}
