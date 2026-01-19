using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllPaisesQueryHandler(IPaisRepository repository, IMapper mapper) : IRequestHandler<GetAllPaisesQuery, IEnumerable<PaisResponseDto>>
{
    private readonly IPaisRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IEnumerable<PaisResponseDto>> Handle(GetAllPaisesQuery request, CancellationToken cancellationToken)
    {
        // 👇 Esto lanza OperationCanceledException si el token ya está cancelado
        cancellationToken.ThrowIfCancellationRequested();
        
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PaisResponseDto>>(entities);
    }
}