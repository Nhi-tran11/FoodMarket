using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service
{
    public class PendingOrderCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PendingOrderCleanupService> _logger;
        // Run cleanup every hour
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public PendingOrderCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<PendingOrderCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PendingOrderCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredPendingOrdersAsync();
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CleanupExpiredPendingOrdersAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var cutoff = DateTime.UtcNow.AddHours(-24);

                var expiredOrders = await dbContext.Orders
                    .Where(o => o.Status == "pending" && o.CreatedAt < cutoff)
                    .ToListAsync();

                if (expiredOrders.Count == 0)
                {
                    return;
                }

                // Release any referral codes locked by these orders so they can be reused
                var orderIds = expiredOrders.Select(o => o.UsedReferalCodeId).Where(id => id != null).ToList();
                if (orderIds.Any())
                {
                    var lockedCodes = await dbContext.ReferalCodes
                        .Where(rc => orderIds.Contains(rc.Id) && !rc.IsUsed)
                        .ToListAsync();

                    // Codes were reserved but never paid — leave them unused so someone else can use them
                    // (they are already IsUsed=false since payment never completed)
                }

                dbContext.Orders.RemoveRange(expiredOrders);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Deleted {Count} expired pending orders (older than 24h).",
                    expiredOrders.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired pending orders.");
            }
        }
    }
}
