namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public sealed record Customer360MetricsSnapshot(
    long MergeTotal,
    long MergeFailures,
    long DedupeQueryTotal,
    long ConsentEventTotal,
    long ConsentGrantedTotal,
    long ConsentRevokedTotal,
    long AuditOutboxEnqueueTotal,
    long AuditOutboxDispatchSuccessTotal,
    long AuditOutboxDispatchFailureTotal,
    long ConcurrencyConflictTotal,
    long ConcurrencyFailureTotal,
    DateTimeOffset? LastOutboxDispatchAtUtc,
    DateTimeOffset? LastConcurrencyConflictAtUtc
);
