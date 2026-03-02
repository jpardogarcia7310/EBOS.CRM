using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.Operations;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public sealed class OperationalReadinessController(
    CrmDbContext dbContext,
    ICustomer360Metrics metrics,
    IOptions<OperationalReadinessOptions> options) : ControllerBase
{
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(Customer360OperationalDashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = metrics.GetSnapshot();
        var outboxPending = await dbContext.AuditOutboxMessages.CountAsync(x => x.ProcessedAt == null, cancellationToken);
        var outboxFailed = await dbContext.AuditOutboxMessages.CountAsync(
            x => x.ProcessedAt != null && x.LastError != null,
            cancellationToken);

        return Ok(new Customer360OperationalDashboardResponse(
            Snapshot: snapshot,
            OutboxPending: outboxPending,
            OutboxFailed: outboxFailed));
    }

    [HttpGet("alerts")]
    [ProducesResponseType(typeof(Customer360OperationalAlertsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertsAsync(CancellationToken cancellationToken = default)
    {
        var cfg = options.Value;
        var snapshot = metrics.GetSnapshot();

        var outboxPending = await dbContext.AuditOutboxMessages.CountAsync(x => x.ProcessedAt == null, cancellationToken);
        var outboxFailed = await dbContext.AuditOutboxMessages.CountAsync(
            x => x.ProcessedAt != null && x.LastError != null,
            cancellationToken);
        var staleDispatch = snapshot.LastOutboxDispatchAtUtc.HasValue &&
                            DateTimeOffset.UtcNow - snapshot.LastOutboxDispatchAtUtc.Value >
                            TimeSpan.FromMinutes(Math.Max(1, cfg.OutboxDispatchStaleMinutesThreshold));

        var response = new Customer360OperationalAlertsResponse(
            OutboxPendingWarning: outboxPending >= cfg.OutboxPendingWarningThreshold,
            OutboxPendingCritical: outboxPending >= cfg.OutboxPendingCriticalThreshold,
            OutboxFailedCritical: outboxFailed >= cfg.OutboxFailedCriticalThreshold,
            OutboxDispatchStale: staleDispatch && outboxPending > 0,
            ConcurrencyCritical: snapshot.ConcurrencyFailureTotal >= cfg.ConcurrencyFailuresCriticalThreshold);

        return Ok(response);
    }
}

public sealed record Customer360OperationalDashboardResponse(
    Customer360MetricsSnapshot Snapshot,
    int OutboxPending,
    int OutboxFailed);

public sealed record Customer360OperationalAlertsResponse(
    bool OutboxPendingWarning,
    bool OutboxPendingCritical,
    bool OutboxFailedCritical,
    bool OutboxDispatchStale,
    bool ConcurrencyCritical);
