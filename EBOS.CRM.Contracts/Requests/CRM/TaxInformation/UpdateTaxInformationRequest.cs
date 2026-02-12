namespace EBOS.CRM.Contracts.Requests.CRM.TaxInformation;

public sealed record UpdateTaxInformationRequest(
    long Id,
    long TenantId,
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId
);
