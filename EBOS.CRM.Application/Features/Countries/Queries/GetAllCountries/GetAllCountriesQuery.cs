using EBOS.CRM.Application.Features.Countries.Dto;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery() : IRequest<IEnumerable<CountryDto>>;