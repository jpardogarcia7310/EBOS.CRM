namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record OpportunityResponse(
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
    string? CloseReason,
    bool Active
);
