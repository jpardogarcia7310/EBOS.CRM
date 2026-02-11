using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetAllStatuses;

public record GetAllStatusesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PagedResult<StatusResponse>>;








