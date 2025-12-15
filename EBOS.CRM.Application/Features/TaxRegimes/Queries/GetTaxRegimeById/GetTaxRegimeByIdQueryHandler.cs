using EBOS.CRM.Application.Features.TaxRegimes.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.TaxRegimes.Queries.GetTaxRegimeById;

public class GetTaxRegimeByIdQueryHandler(ITaxRegimeRepository repository, IMapper mapper) : IRequestHandler<GetTaxRegimeByIdQuery, TaxRegimeResponseDto?>
{
    public async Task<TaxRegimeResponseDto?> Handle(GetTaxRegimeByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<TaxRegimeResponseDto>(entity);
    }
}