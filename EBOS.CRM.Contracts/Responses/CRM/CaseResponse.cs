namespace EBOS.CRM.Contracts.Responses.CRM;

public record CaseResponse(
    long Id,
    long TenantId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    long OwnerUserId,
    long QueueId,
    long SlaId,
    DateTime? DueAt,
    DateTime? ClosedAt,
    bool Active
);
