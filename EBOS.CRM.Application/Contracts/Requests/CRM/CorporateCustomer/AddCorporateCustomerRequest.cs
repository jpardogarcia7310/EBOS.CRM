namespace EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;

public record AddCorporateCustomerRequest(
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId,
    string LegalName,
    string TaxIdentification
);
