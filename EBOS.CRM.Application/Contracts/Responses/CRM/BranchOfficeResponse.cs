namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record BranchOfficeResponse(
    long Id,
    string Name,
    string PhoneNumber,
    long CorporateCustomerId,
    bool Active
);
