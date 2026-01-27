using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandler(ICountryRepository repository, IMapper mapper) 
    : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryResponse>>
{
    private readonly ICountryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IEnumerable<CountryResponse>> Handle(GetAllCountriesQuery request, 
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();
        
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<CountryResponse>>(entities);
    }
}