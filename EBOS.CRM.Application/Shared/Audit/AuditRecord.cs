namespace EBOS.CRM.Application.Shared.Audit;

public sealed record AuditRecord(
    long Id,
    long UserId,
    DateTimeOffset TimeStamp,
    string Action,
    string Entity,
    long RegisterId,
    string? OldValues,
    string? NewValues,
    string CorrelationId
);
