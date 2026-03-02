using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditOutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    ILogger<AuditOutboxDispatcher> logger,
    IOptions<AuditOutboxOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(5, options.Value.DispatchIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var outbox = scope.ServiceProvider.GetRequiredService<IAuditOutboxService>();
                await outbox.DispatchPendingAsync(stoppingToken);
            }
            catch
            {
                logger.LogWarning("Audit outbox dispatcher iteration failed. Dispatcher will continue with next cycle.");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}
