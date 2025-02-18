namespace MyStockDashboard.Models;

public class StockPriceHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Price { get; set; }
    
    public string Currency { get; set; }
}