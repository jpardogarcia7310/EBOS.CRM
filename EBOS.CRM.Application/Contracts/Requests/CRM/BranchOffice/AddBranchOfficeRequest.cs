namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public record AddBranchOfficeRequest(
    long TenantId,
    string Name,
    string PhoneNumber,
    long CorporateCustomerId
);
