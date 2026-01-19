using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;

public class GetPaisByIdQueryHandler(IPaisRepository repository, IMapper mapper) : IRequestHandler<GetPaisByIdQuery, PaisResponseDto?>
{
    private readonly IPaisRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PaisResponseDto?> Handle(GetPaisByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();
        
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<PaisResponseDto>(entity);
    }
}