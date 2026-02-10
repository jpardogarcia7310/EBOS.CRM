namespace EBOS.CRM.Application.Contracts.Requests.CRM.Lead;

public record AddLeadRequest(
    long TenantId,
    string Source,
    string Status,
    long OwnerUserId,
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    decimal? EstimatedValue,
    string? Notes
);
