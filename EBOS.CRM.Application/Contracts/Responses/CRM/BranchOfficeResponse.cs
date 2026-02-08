namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record BranchOfficeResponse(
    long Id,
    long TenantId,
    string Name,
    string PhoneNumber,
    long CorporateCustomerId,
    bool Active
);
