using MapsterMapper;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandler(ICountryRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCountriesQuery, PagedResult<CountryResponse>>
{
    private readonly ICountryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<CountryResponse>> Handle(GetAllCountriesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize,
            cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CountryResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<CountryResponse>(items, total);
    }
}









