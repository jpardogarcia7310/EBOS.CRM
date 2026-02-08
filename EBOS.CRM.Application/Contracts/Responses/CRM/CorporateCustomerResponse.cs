namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CorporateCustomerResponse(
    long Id,
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    string LegalName,
    string TaxIdentification,
    bool Active
);
