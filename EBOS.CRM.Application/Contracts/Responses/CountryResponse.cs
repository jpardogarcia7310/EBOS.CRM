namespace EBOS.CRM.Application.Contracts.Responses;

public record CountryResponse(
    long Id,
    string Name,
    string Iso31661A2Code,
    string Iso31661A3Code,
    string Iso31661NumCode,
    string Domain,
    string Currency,
    string CurrencyCode,
    string InternationalPhoneCode
);