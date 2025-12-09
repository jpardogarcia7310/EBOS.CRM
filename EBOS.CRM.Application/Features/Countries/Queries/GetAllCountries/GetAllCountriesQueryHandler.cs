using AutoMapper;
using EBOS.CRM.Application.Features.Countries.Dto;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandler(ICountryRepository repository, IMapper mapper) : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
{
    public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<IEnumerable<CountryDto>>(await repository.GetAllAsync(cancellationToken));
    }
}