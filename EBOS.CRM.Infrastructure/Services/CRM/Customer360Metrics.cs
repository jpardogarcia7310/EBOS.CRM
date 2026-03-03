using System.Diagnostics.Metrics;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Observability;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public sealed class Customer360Metrics : ICustomer360Metrics
{
    private static readonly Meter Meter = new(TelemetryNames.Customer360Meter, "1.0.0");
    private static readonly Counter<long> MergeCounter = Meter.CreateCounter<long>("customer360.merge.total");
    private static readonly Counter<long> DedupeCounter = Meter.CreateCounter<long>("customer360.dedupe.query.total");
    private static readonly Counter<long> ConsentCounter = Meter.CreateCounter<long>("customer360.consent.event.total");
    private static readonly Counter<long> AuditOutboxCounter = Meter.CreateCounter<long>("customer360.audit.outbox.total");
    private static readonly Counter<long> ConcurrencyCounter = Meter.CreateCounter<long>("customer360.concurrency.total");

    private long _mergeTotal;
    private long _mergeFailures;
    private long _dedupeQueryTotal;
    private long _consentEventTotal;
    private long _consentGrantedTotal;
    private long _consentRevokedTotal;
    private long _auditOutboxEnqueueTotal;
    private long _auditOutboxDispatchSuccessTotal;
    private long _auditOutboxDispatchFailureTotal;
    private long _concurrencyConflictTotal;
    private long _concurrencyFailureTotal;
    private long _lastOutboxDispatchTicks;
    private long _lastConcurrencyConflictTicks;

    public void RecordMerge(long tenantId, int mergedCount, bool success)
    {
        MergeCounter.Add(1,
            new("tenant_id", tenantId),
            new("merged_count", mergedCount),
            new("success", success));
        Interlocked.Increment(ref _mergeTotal);
        if (!success)
        {
            Interlocked.Increment(ref _mergeFailures);
        }
    }

    public void RecordDedupeQuery(long tenantId, int candidateCount, int scoreThreshold)
    {
        DedupeCounter.Add(1,
            new("tenant_id", tenantId),
            new("candidate_count", candidateCount),
            new("score_threshold", scoreThreshold));
        Interlocked.Increment(ref _dedupeQueryTotal);
    }

    public void RecordConsentEvent(long tenantId, string consentType, bool granted)
    {
        ConsentCounter.Add(1,
            new("tenant_id", tenantId),
            new("consent_type", consentType),
            new("granted", granted));
        Interlocked.Increment(ref _consentEventTotal);
        if (granted)
        {
            Interlocked.Increment(ref _consentGrantedTotal);
        }
        else
        {
            Interlocked.Increment(ref _consentRevokedTotal);
        }
    }

    public void RecordAuditOutboxEnqueue(string operation)
    {
        AuditOutboxCounter.Add(1,
            new("operation", operation),
            new("event", "enqueue"));
        Interlocked.Increment(ref _auditOutboxEnqueueTotal);
    }

    public void RecordAuditOutboxDispatch(string operation, bool success)
    {
        AuditOutboxCounter.Add(1,
            new("operation", operation),
            new("event", "dispatch"),
            new("success", success));
        if (success)
        {
            Interlocked.Increment(ref _auditOutboxDispatchSuccessTotal);
        }
        else
        {
            Interlocked.Increment(ref _auditOutboxDispatchFailureTotal);
        }

        Interlocked.Exchange(ref _lastOutboxDispatchTicks, DateTimeOffset.UtcNow.UtcTicks);
    }

    public void RecordConcurrencyConflict(bool exhaustedRetries)
    {
        ConcurrencyCounter.Add(1, [new("exhausted_retries", (object?)exhaustedRetries)]);
        Interlocked.Increment(ref _concurrencyConflictTotal);
        if (exhaustedRetries)
        {
            Interlocked.Increment(ref _concurrencyFailureTotal);
        }

        Interlocked.Exchange(ref _lastConcurrencyConflictTicks, DateTimeOffset.UtcNow.UtcTicks);
    }

    public Customer360MetricsSnapshot GetSnapshot()
    {
        var lastOutboxTicks = Interlocked.Read(ref _lastOutboxDispatchTicks);
        var lastConcurrencyTicks = Interlocked.Read(ref _lastConcurrencyConflictTicks);

        return new Customer360MetricsSnapshot(
            MergeTotal: Interlocked.Read(ref _mergeTotal),
            MergeFailures: Interlocked.Read(ref _mergeFailures),
            DedupeQueryTotal: Interlocked.Read(ref _dedupeQueryTotal),
            ConsentEventTotal: Interlocked.Read(ref _consentEventTotal),
            ConsentGrantedTotal: Interlocked.Read(ref _consentGrantedTotal),
            ConsentRevokedTotal: Interlocked.Read(ref _consentRevokedTotal),
            AuditOutboxEnqueueTotal: Interlocked.Read(ref _auditOutboxEnqueueTotal),
            AuditOutboxDispatchSuccessTotal: Interlocked.Read(ref _auditOutboxDispatchSuccessTotal),
            AuditOutboxDispatchFailureTotal: Interlocked.Read(ref _auditOutboxDispatchFailureTotal),
            ConcurrencyConflictTotal: Interlocked.Read(ref _concurrencyConflictTotal),
            ConcurrencyFailureTotal: Interlocked.Read(ref _concurrencyFailureTotal),
            LastOutboxDispatchAtUtc: lastOutboxTicks == 0 ? null : new DateTimeOffset(lastOutboxTicks, TimeSpan.Zero),
            LastConcurrencyConflictAtUtc: lastConcurrencyTicks == 0 ? null : new DateTimeOffset(lastConcurrencyTicks, TimeSpan.Zero));
    }
}
