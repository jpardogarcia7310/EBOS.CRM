namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record AddBranchOfficeRequest(
    string Name,
    string PhoneNumber,
    long CorporateCustomerId);
