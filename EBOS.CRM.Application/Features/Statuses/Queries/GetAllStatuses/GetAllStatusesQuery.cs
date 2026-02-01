using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public record GetAllStatusesQuery(PagedQueryRequest Query) : IRequest<PagedResponse<StatusResponse>>;



