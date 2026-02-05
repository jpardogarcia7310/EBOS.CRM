namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public sealed record UpdateBranchOfficeRequest(
    long Id,
    long TenantId,
    string Name,
    string PhoneNumber,
    long CorporateCustomerId);
