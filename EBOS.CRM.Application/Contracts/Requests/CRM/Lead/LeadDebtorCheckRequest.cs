namespace EBOS.CRM.Application.Contracts.Requests.CRM.Lead;

public sealed record LeadDebtorCheckRequest(
    long TenantId,
    string? Email,
    string? Phone,
    string? CompanyName,
    string? ContactName
);
