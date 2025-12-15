using EBOS.CRM.Application.Features.TaxRegimes.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.TaxRegimes.Queries.GetAllTaxRegimes;

public class GetAllTaxRegimesQueryHandler(ITaxRegimeRepository repository, IMapper mapper) : IRequestHandler<GetAllTaxRegimesQuery, IEnumerable<TaxRegimeResponseDto>>
{
    public async Task<IEnumerable<TaxRegimeResponseDto>> Handle(GetAllTaxRegimesQuery request, CancellationToken cancellationToken)
    {
        // 👇 Esto lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<TaxRegimeResponseDto>>(entities);
    }
}