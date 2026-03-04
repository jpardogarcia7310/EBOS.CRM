using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetAllChannelCountries;

public class GetAllChannelCountriesQueryHandler(IChannelCountryRepository repository, IMapper mapper)
    : IRequestHandler<GetAllChannelCountriesQuery, PagedResult<ChannelCountryResponse>>
{
    private readonly IChannelCountryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<ChannelCountryResponse>> Handle(GetAllChannelCountriesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<ChannelCountryResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<ChannelCountryResponse>(items, total);
    }
}
