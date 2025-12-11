using AutoMapper;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;

public class GetCountryByIdQueryHandler(ICountryRepository repository, IMapper mapper) : IRequestHandler<GetCountryByIdQuery, CountryResponseDto?>
{
    public async Task<CountryResponseDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<CountryResponseDto>(await repository.GetByIdAsync(request.Id, cancellationToken));
    }
}