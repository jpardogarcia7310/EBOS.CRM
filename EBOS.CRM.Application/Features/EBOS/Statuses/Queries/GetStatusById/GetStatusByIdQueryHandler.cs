using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryHandler(IStatusRepository repository, IMapper mapper)
    : IRequestHandler<GetStatusByIdQuery, StatusResponse?>
{
    public async Task<StatusResponse?> Handle(GetStatusByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 It throws OperationCancelledException if the token has already been canceled
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<StatusResponse>(entity);
    }
}



