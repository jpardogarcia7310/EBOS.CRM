namespace EBOS.CRM.Contracts.Requests.CRM.Lead;

public sealed record QualifyLeadRequest(
    long TenantId,
    string? Notes
);
