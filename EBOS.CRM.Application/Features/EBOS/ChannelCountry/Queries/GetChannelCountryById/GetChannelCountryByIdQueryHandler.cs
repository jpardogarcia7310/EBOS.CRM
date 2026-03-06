using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;

public class GetChannelCountryByIdQueryHandler(IChannelCountryRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetChannelCountryByIdQuery, ChannelCountryResponse?>
{
    private readonly IChannelCountryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<ChannelCountryResponse?> Handle(GetChannelCountryByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = referenceLookupService is null
            ? await _repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetChannelCountryByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<ChannelCountryResponse>(entity);
    }
}
