namespace EBOS.CRM.Contracts.Requests.CRM.Lead;

public sealed record UpdateLeadRequest(
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
    string? Notes
);
