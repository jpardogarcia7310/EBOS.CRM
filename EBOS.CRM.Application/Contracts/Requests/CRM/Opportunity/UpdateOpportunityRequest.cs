namespace EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;

public sealed record UpdateOpportunityRequest(
    long Id,
    long TenantId,
    string Name,
    long StageId,
    long OwnerUserId,
    long CustomerId,
    DateTime? ExpectedCloseDate,
    decimal Amount,
    decimal Probability,
    string? Source,
    long? SourceLeadId,
    string? CloseReason
);
