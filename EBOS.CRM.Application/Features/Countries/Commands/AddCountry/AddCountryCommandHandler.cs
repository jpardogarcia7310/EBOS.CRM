using AutoMapper;
using EBOS.CRM.Application.Features.Countries.Dto;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Commands.AddCountry;

public class AddCountryCommandHandler(ICountryRepository repository, IMapper mapper) : IRequestHandler<AddCountryCommand, CountryDto>
{
    public async Task<CountryDto> Handle(AddCountryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Country
        {
            Name = request.Name,
            Iso31661A2Code = request.Iso31661A2Code,
            Iso31661A3Code = request.Iso31661A3Code,
            Iso31661NumCode = request.Iso31661NumCode,
            Domain = request.Domain,
            Currency = request.Currency,
            CurrencyCode = request.CurrencyCode,
            InternationalPhoneCode = request.InternationalPhoneCode
        };

        var created = await repository.AddAsync(entity, cancellationToken);
        created.Id = await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<CountryDto>(created);
    }
}