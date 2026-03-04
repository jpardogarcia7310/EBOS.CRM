namespace EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;

public record FindCustomerDuplicatesRequest(
    long TenantId,
    string? Email,
    string? Phone,
    string? TaxId,
    string? IdentificationNumber
);
