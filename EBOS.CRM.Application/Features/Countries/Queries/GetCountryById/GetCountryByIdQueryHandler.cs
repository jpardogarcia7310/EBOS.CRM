using AutoMapper;
using EBOS.CRM.Application.Features.Countries.Dto;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;

public class GetCountryByIdQueryHandler(ICountryRepository repository, IMapper mapper) : IRequestHandler<GetCountryByIdQuery, CountryDto?>
{
    public async Task<CountryDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<CountryDto>(await repository.GetByIdAsync(request.Id, cancellationToken));
    }
}