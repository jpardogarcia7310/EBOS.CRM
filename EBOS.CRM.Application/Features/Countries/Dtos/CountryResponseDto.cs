namespace EBOS.CRM.Application.Features.Countries.Dtos;

public record CountryResponseDto(
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