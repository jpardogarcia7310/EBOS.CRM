namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record TaxInformationResponse(
    long Id,
    long TenantId,
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId,
    bool Active
);
