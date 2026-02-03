namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public sealed record UpdateBranchOfficeRequest(
    long Id,
    string Name,
    string PhoneNumber,
    long CorporateCustomerId);
