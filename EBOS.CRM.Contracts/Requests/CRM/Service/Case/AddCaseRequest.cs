namespace EBOS.CRM.Contracts.Requests.CRM.Service.Case;

public sealed record AddCaseRequest(
    long TenantId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    long OwnerUserId,
    long QueueId,
    long SlaId,
    DateTime? DueAt
);
