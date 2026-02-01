using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery(PagedQueryRequest Query) : IRequest<PagedResponse<CountryResponse>>;



