namespace EBOS.CRM.Domain.Interfaces.Services.Models;

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