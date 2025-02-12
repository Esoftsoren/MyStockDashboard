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
}