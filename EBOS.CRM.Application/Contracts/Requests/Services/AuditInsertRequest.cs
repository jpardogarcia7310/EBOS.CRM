namespace EBOS.CRM.Application.Contracts.Requests.Services;

public sealed record AuditInsertRequest(
    long UserId,
    DateTimeOffset TimeStamp,
    string Action,
    string Entity,
    long RegisterId,
    string? OldValues,
    string? NewValues,
    string CorrelationId);
