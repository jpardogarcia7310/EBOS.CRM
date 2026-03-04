using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;

public class GetAllChannelTypesQueryHandler(IChannelTypeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllChannelTypesQuery, PagedResult<ChannelTypeResponse>>
{
    private readonly IChannelTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<ChannelTypeResponse>> Handle(GetAllChannelTypesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<ChannelTypeResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<ChannelTypeResponse>(items, total);
    }
}
