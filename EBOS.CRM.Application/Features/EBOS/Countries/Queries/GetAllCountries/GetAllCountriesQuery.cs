using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PagedResult<CountryResponse>>;








