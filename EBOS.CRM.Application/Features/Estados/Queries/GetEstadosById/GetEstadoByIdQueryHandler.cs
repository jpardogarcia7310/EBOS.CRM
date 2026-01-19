using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;

public class GetEstadoByIdQueryHandler(IEstadoRepository repository, IMapper mapper) : IRequestHandler<GetEstadoByIdQuery, EstadoResponseDto?>
{
    public async Task<EstadoResponseDto?> Handle(GetEstadoByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<EstadoResponseDto>(entity);
    }
}