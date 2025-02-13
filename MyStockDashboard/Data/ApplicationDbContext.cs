using Microsoft.EntityFrameworkCore;
using MyStockDashboard.Models;

namespace MyStockDashboard.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<PressRelease> PressReleases { get; set; }
    public DbSet<StockPriceHistory> StockPriceHistories { get; set; }
    public DbSet<UserPortfolioStock> UserPortfolioStocks { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>()
            .Property(s => s.StockPrice)
            .HasPrecision(18, 4);
        
        modelBuilder.Entity<StockPriceHistory>()
            .Property(s => s.Price)
            .HasPrecision(18, 4);
            
        // Optionally configure the UserPortfolioStock symbol column:
        modelBuilder.Entity<UserPortfolioStock>()
            .Property(s => s.Symbol)
            .HasColumnType("nvarchar(max)"); // or choose a specific length

        base.OnModelCreating(modelBuilder);
    }

}