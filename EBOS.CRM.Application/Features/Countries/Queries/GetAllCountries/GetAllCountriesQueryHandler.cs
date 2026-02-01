using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandler(ICountryRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCountriesQuery, PagedResponse<CountryResponse>>
{
    private readonly ICountryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<CountryResponse>> Handle(GetAllCountriesQuery request,
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CountryResponse>>(result.Items);
        return new PagedResponse<CountryResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}



