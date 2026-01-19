using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllEstadosQueryHandler(IEstadoRepository repository, IMapper mapper) : IRequestHandler<GetAllEstadosQuery, IEnumerable<EstadoResponseDto>>
{
    public async Task<IEnumerable<EstadoResponseDto>> Handle(GetAllEstadosQuery request, CancellationToken cancellationToken)
    {
        // 👇 Esto lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<EstadoResponseDto>>(entities);
    }
}