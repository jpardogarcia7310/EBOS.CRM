using System.Diagnostics.Metrics;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public sealed class Customer360Metrics : ICustomer360Metrics
{
    private static readonly Meter Meter = new("EBOS.CRM.Customer360", "1.0.0");
    private static readonly Counter<long> MergeCounter = Meter.CreateCounter<long>("customer360.merge.total");
    private static readonly Counter<long> DedupeCounter = Meter.CreateCounter<long>("customer360.dedupe.query.total");
    private static readonly Counter<long> ConsentCounter = Meter.CreateCounter<long>("customer360.consent.event.total");
    private static readonly Counter<long> AuditOutboxCounter = Meter.CreateCounter<long>("customer360.audit.outbox.total");

    public void RecordMerge(long tenantId, int mergedCount, bool success)
        => MergeCounter.Add(1,
            new("tenant_id", tenantId),
            new("merged_count", mergedCount),
            new("success", success));

    public void RecordDedupeQuery(long tenantId, int candidateCount, int scoreThreshold)
        => DedupeCounter.Add(1,
            new("tenant_id", tenantId),
            new("candidate_count", candidateCount),
            new("score_threshold", scoreThreshold));

    public void RecordConsentEvent(long tenantId, string consentType, bool granted)
        => ConsentCounter.Add(1,
            new("tenant_id", tenantId),
            new("consent_type", consentType),
            new("granted", granted));

    public void RecordAuditOutboxEnqueue(string operation)
        => AuditOutboxCounter.Add(1,
            new("operation", operation),
            new("event", "enqueue"));

    public void RecordAuditOutboxDispatch(string operation, bool success)
        => AuditOutboxCounter.Add(1,
            new("operation", operation),
            new("event", "dispatch"),
            new("success", success));
}
