using AutoMapper;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandler(ICountryRepository repository, IMapper mapper) : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryResponseDto>>
{
    public async Task<IEnumerable<CountryResponseDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<IEnumerable<CountryResponseDto>>(await repository.GetAllAsync(cancellationToken));
    }
}