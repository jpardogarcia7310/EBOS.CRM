namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public record UpdateBranchOfficeRequest(
    string Name,
    string PhoneNumber,
    long CorporateCustomerId
);
