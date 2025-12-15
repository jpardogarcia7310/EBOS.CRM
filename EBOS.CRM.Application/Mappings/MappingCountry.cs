using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingCountry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Country, CountryResponseDto>();

        config.NewConfig<CountryResponseDto, Country>();
    }
}