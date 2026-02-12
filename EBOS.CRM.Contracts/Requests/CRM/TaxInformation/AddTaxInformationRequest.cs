namespace EBOS.CRM.Contracts.Requests.CRM.TaxInformation;

public record AddTaxInformationRequest(
    long TenantId,
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId
);
