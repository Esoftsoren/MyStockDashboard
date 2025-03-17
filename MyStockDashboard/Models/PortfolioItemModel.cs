namespace MyStockDashboard.Models;

public class PortfolioItemModel
{
    public string Symbol { get; set; }
    public double Quantity { get; set; }
        
    /// <summary>
    /// Purchase price as entered by the user (in the stock’s local currency).
    /// </summary>
    public decimal PurchasePrice { get; set; }
        
    /// <summary>
    /// Purchase price converted to USD.
    /// </summary>
    public decimal? PurchasePriceUSD { get; set; }
        
    /// <summary>
    /// Latest market price converted to USD.
    /// </summary>
    public decimal? LatestPrice { get; set; }
}