using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllStatusesQueryHandler(IStatusRepository repository, IMapper mapper) : 
    IRequestHandler<GetAllStatusesQuery, IEnumerable<StatusResponseDto>>
{
    public async Task<IEnumerable<StatusResponseDto>> Handle(GetAllStatusesQuery request, 
        CancellationToken cancellationToken)
    {
        // 👇 Esto lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<StatusResponseDto>>(entities);
    }
}