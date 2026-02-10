namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public sealed record PatchBranchOfficeRequest(
    long TenantId,
    string? Name,
    string? PhoneNumber,
    long? CorporateCustomerId
);
