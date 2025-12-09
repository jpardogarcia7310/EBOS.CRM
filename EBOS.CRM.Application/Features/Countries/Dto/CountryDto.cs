namespace EBOS.CRM.Application.Features.Countries.Dto;

public record CountryDto(
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