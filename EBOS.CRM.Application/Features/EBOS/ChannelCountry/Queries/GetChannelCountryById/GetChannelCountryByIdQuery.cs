using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;

public record GetChannelCountryByIdQuery(long Id) : IRequest<ChannelCountryResponse?>;
