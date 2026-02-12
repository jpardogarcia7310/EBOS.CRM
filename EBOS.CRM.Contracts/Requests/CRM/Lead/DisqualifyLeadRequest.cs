namespace EBOS.CRM.Contracts.Requests.CRM.Lead;

public sealed record DisqualifyLeadRequest(
    long TenantId,
    string Reason
);
