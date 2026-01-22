using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllStatusesQueryHandler(IStatusRepository repository, IMapper mapper) : 
    IRequestHandler<GetAllStatusesQuery, IEnumerable<StatusResponse>>
{
    public async Task<IEnumerable<StatusResponse>> Handle(GetAllStatusesQuery request, 
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token has already been cancelled.
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<StatusResponse>>(entities);
    }
}