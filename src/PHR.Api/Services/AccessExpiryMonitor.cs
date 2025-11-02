using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PHR.Api.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PHR.Api.Services
{
    public class AccessExpiryMonitor : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<AccessExpiryMonitor> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

        public AccessExpiryMonitor(IServiceProvider sp, ILogger<AccessExpiryMonitor> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AccessExpiryMonitor started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var now = DateTime.UtcNow;
                    var expired = await db.AccessRequests
                        .Where(a => a.Status == "Approved" && a.EndDateTime < now)
                        .ToListAsync(stoppingToken);

                    foreach (var req in expired)
                    {
                        req.Status = "Expired";
                    }

                    if (expired.Count > 0)
                    {
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Marked {expired.Count} access requests as Expired.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking expired access requests.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
