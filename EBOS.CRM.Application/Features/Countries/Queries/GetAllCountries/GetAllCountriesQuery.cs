using EBOS.CRM.Application.Contracts.Responses;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery : IRequest<IEnumerable<CountryResponse>>;