using EBOS.CRM.Api.Options;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Infrastructure.HealthChecks;

public sealed class Customer360OperationalReadinessHealthCheck(
    CrmDbContext dbContext,
    ICustomer360Metrics metrics,
    IOptions<OperationalReadinessOptions> options)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var cfg = options.Value;
        var pendingOutbox = await dbContext.AuditOutboxMessages.CountAsync(x => x.ProcessedAt == null, cancellationToken);
        var failedOutbox = await dbContext.AuditOutboxMessages.CountAsync(
            x => x.ProcessedAt != null && x.LastError != null,
            cancellationToken);

        var snapshot = metrics.GetSnapshot();
        var staleDispatch = snapshot.LastOutboxDispatchAtUtc.HasValue &&
                            DateTimeOffset.UtcNow - snapshot.LastOutboxDispatchAtUtc.Value >
                            TimeSpan.FromMinutes(Math.Max(1, cfg.OutboxDispatchStaleMinutesThreshold));

        var data = new Dictionary<string, object>
        {
            ["outbox.pending"] = pendingOutbox,
            ["outbox.failed"] = failedOutbox,
            ["outbox.lastDispatchAtUtc"] = snapshot.LastOutboxDispatchAtUtc?.ToString("O") ?? "n/a",
            ["outbox.dispatchStale"] = staleDispatch,
            ["concurrency.conflicts.total"] = snapshot.ConcurrencyConflictTotal,
            ["concurrency.failures.total"] = snapshot.ConcurrencyFailureTotal
        };

        if (pendingOutbox >= cfg.OutboxPendingCriticalThreshold ||
            failedOutbox >= cfg.OutboxFailedCriticalThreshold ||
            snapshot.ConcurrencyFailureTotal >= cfg.ConcurrencyFailuresCriticalThreshold ||
            (staleDispatch && pendingOutbox > 0))
        {
            return HealthCheckResult.Unhealthy(
                "Customer360 operability thresholds exceeded.",
                data: data);
        }

        if (pendingOutbox >= cfg.OutboxPendingWarningThreshold || staleDispatch)
        {
            return HealthCheckResult.Degraded(
                "Customer360 operability warning thresholds exceeded.",
                data: data);
        }

        return HealthCheckResult.Healthy("Customer360 operational readiness is healthy.", data);
    }
}
