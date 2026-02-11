using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetQueueById;

public class GetQueueByIdQueryHandler(IQueueRepository repository, IMapper mapper)
    : IRequestHandler<GetQueueByIdQuery, QueueResponse?>
{
    public async Task<QueueResponse?> Handle(GetQueueByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<QueueResponse>(entity);
    }
}
