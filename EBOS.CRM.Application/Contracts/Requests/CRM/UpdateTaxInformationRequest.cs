namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record UpdateTaxInformationRequest(
    long Id,
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId);
