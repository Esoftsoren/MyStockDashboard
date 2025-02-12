using HtmlAgilityPack;
using MyStockDashboard.Data;

namespace MyStockDashboard.Services;

public class PressReleaseScraperService : BackgroundService
    {
        private readonly ILogger<PressReleaseScraperService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _scrapeInterval = TimeSpan.FromMinutes(15);

        public PressReleaseScraperService(ILogger<PressReleaseScraperService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PressReleaseScraperService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ScrapePressReleasesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during scraping.");
                }

                await Task.Delay(_scrapeInterval, stoppingToken);
            }

            _logger.LogInformation("PressReleaseScraperService is stopping.");
        }

        private async Task ScrapePressReleasesAsync(CancellationToken stoppingToken)
        {
            // Create a new scope to access scoped services like ApplicationDbContext.
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Example: Scrape Hansa Biopharma press releases.
            var url = "https://www.hansabiopharma.com/media/press-releases/";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Implement parsing logic:
            // - Locate the nodes that contain press release dates, titles, URLs, etc.
            // - Compare the dates (and timestamps) with the current date.
            // - If a new press release is found, save it to the database.
            // You might need to tailor the logic for each website.
            // For now, log that you have scraped the site.
            _logger.LogInformation("Scraped press releases from Hansa Biopharma.");

            // Repeat similar logic for the other websites:
            // https://www.supermicro.com/en/newsroom/pressreleases
            // https://www.anavex.com/investor-material

            // (Don’t forget to call dbContext.SaveChangesAsync() after inserting new records.)
        }
    }