namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record AddTaxInformationRequest(
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId);
