using AutoMapper;
using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Commands.UpdateCountry;

public sealed class UpdateCountryCommandHandler(ICountryRepository repository, IMapper mapper) : IRequestHandler<UpdateCountryCommand, CountryResponseDto>
{
    public async Task<CountryResponseDto> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"Country with id {request.Id} not found.");

        // Aplicar cambios
        country.Name = request.Name;
        country.Iso31661A2Code = request.Iso31661A2Code;
        country.Iso31661A3Code = request.Iso31661A3Code;
        country.Iso31661NumCode = request.Iso31661NumCode;
        country.Domain = request.Domain;
        country.Currency = request.Currency;
        country.CurrencyCode = request.CurrencyCode;
        country.InternationalPhoneCode = request.InternationalPhoneCode;

        await repository.UpdateAsync(country, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<CountryResponseDto>(country);
    }
}