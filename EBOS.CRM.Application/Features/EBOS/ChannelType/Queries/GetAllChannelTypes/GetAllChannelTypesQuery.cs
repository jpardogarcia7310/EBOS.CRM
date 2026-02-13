using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;

public record GetAllChannelTypesQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<ChannelTypeResponse>>;
