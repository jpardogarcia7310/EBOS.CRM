namespace EBOS.CRM.Application.Contracts.Requests.CRM.Lead;

public sealed record DisqualifyLeadRequest(
    long TenantId,
    string Reason
);
