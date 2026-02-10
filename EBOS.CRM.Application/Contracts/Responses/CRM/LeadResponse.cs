namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record LeadResponse(
    long Id,
    long TenantId,
    string Source,
    string Status,
    long OwnerUserId,
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    decimal? EstimatedValue,
    string? Notes,
    long? ConvertedOpportunityId,
    bool Active
);
