namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;

public record AddBranchOfficeRequest(
    string Name,
    string PhoneNumber,
    long CorporateCustomerId
);
