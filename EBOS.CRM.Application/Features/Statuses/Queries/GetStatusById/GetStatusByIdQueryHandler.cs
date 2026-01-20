using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryHandler(IStatusRepository repository, IMapper mapper) 
    : IRequestHandler<GetStatusByIdQuery, StatusResponseDto?>
{
    public async Task<StatusResponseDto?> Handle(GetStatusByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<StatusResponseDto>(entity);
    }
}