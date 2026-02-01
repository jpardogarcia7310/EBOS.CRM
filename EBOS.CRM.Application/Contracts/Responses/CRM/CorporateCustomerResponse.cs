namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CorporateCustomerResponse(
    long Id,
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId,
    string LegalName,
    string TaxIdentification,
    bool Active
);
