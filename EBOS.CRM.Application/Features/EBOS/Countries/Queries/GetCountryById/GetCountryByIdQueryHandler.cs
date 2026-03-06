using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetCountryById;

public class GetCountryByIdQueryHandler(ICountryRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetCountryByIdQuery, CountryResponse?>
{
    private readonly ICountryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<CountryResponse?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Throws OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var entity = referenceLookupService is null
            ? await _repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetCountryByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<CountryResponse>(entity);
    }
}



