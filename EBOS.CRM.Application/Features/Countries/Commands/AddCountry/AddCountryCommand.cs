using EBOS.CRM.Application.Features.Countries.Dtos;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Commands.AddCountry;

public record AddCountryCommand(
    string Name,
    string Iso31661A2Code,
    string Iso31661A3Code,
    string Iso31661NumCode,
    string Domain,
    string Currency,
    string CurrencyCode,
    string InternationalPhoneCode
) : IRequest<CountryDto>;