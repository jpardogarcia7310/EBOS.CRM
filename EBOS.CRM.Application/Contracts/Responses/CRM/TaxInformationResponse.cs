namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public sealed record TaxInformationResponse(
    long Id,
    string TaxName,
    string TaxIdentificationNumber,
    long CustomerId,
    bool Active);
