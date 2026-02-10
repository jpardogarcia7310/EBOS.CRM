namespace EBOS.CRM.Application.Contracts.Requests.CRM.Lead;

public sealed record ConvertLeadRequest(
    long TenantId,
    long CustomerId,
    long StageId,
    string OpportunityName,
    decimal Amount,
    decimal Probability,
    DateTime? ExpectedCloseDate,
    string? Notes
);
