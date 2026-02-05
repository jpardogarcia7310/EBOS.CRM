using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

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









