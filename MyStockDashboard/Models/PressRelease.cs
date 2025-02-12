namespace MyStockDashboard.Models;

public class PressRelease
{
    public Guid Id { get; set; }
    
    public string StockName { get; set; }
    
    public DateTime ReleaseDate { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
    public string Headline { get; set; }
    
    public string Source { get; set; }
    
    public string Url { get; set; }
}