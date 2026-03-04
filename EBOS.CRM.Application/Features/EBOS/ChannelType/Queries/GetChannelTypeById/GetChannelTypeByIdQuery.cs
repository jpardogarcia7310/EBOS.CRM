using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;

public record GetChannelTypeByIdQuery(long Id) : IRequest<ChannelTypeResponse?>;
