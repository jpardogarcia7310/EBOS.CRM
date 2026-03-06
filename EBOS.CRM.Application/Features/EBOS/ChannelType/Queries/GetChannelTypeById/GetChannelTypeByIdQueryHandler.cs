using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;

public class GetChannelTypeByIdQueryHandler(IChannelTypeRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetChannelTypeByIdQuery, ChannelTypeResponse?>
{
    private readonly IChannelTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<ChannelTypeResponse?> Handle(GetChannelTypeByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = referenceLookupService is null
            ? await _repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetChannelTypeByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<ChannelTypeResponse>(entity);
    }
}
