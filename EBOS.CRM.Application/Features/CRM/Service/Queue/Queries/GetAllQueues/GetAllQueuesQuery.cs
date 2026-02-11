using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetAllQueues;

public record GetAllQueuesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<QueueResponse>>;
