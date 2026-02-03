using EBOS.CRM.Application.Contracts.Responses;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PagedResult<CountryResponse>>;








