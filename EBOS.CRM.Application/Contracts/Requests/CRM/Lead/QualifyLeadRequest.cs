namespace EBOS.CRM.Application.Contracts.Requests.CRM.Lead;

public sealed record QualifyLeadRequest(
    long TenantId,
    string? Notes
);
