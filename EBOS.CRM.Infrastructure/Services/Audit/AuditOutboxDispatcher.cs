using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditOutboxDispatcher(
    IServiceScopeFactory scopeFactory,
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
                // no-op to keep background dispatcher alive
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}
