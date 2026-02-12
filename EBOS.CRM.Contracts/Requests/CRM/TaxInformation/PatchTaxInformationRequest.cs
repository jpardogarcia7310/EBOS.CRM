namespace EBOS.CRM.Contracts.Requests.CRM.TaxInformation;

public sealed record PatchTaxInformationRequest(
    long TenantId,
    string? TaxName,
    string? TaxIdentificationNumber,
    long? CustomerId
);
