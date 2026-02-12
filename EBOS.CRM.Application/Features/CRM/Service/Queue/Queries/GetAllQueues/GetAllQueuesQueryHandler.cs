using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetAllQueues;

public class GetAllQueuesQueryHandler(IQueueRepository repository, IMapper mapper)
    : IRequestHandler<GetAllQueuesQuery, PagedResult<QueueResponse>>
{
    public async Task<PagedResult<QueueResponse>> Handle(GetAllQueuesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<QueueResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<QueueResponse>(items, total);
    }
}
