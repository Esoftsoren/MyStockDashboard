namespace MyStockDashboard.Models;

public class UserPortfolioStock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; }
    public string CompanyName { get; set; }
}