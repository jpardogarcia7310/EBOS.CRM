using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetAllChannelCountries;

public record GetAllChannelCountriesQuery(int PageNumber, int PageSize)
    : IRequest<PagedResult<ChannelCountryResponse>>;
