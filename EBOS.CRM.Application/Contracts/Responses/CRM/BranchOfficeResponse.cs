namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public sealed record BranchOfficeResponse(
    long Id,
    string Name,
    string PhoneNumber,
    long CorporateCustomerId,
    bool Active);
