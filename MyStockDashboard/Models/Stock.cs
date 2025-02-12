namespace MyStockDashboard.Models;

public class Stock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Symbol { get; set; }
    
    public string CompanyName { get; set; }
    
    public string Exchange { get; set; }
    
    public string Industry { get; set; }
    
    public string CompanyDescription { get; set; }
    
    public string CEO { get; set; }
    
    public decimal StockPrice { get; set; } = new();
    
}