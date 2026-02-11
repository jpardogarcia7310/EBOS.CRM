using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public sealed class MappingCountry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Country, CountryResponse>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.Iso31661A2Code, src => src.Iso31661A2Code)
              .Map(dest => dest.Iso31661A3Code, src => src.Iso31661A3Code)
              .Map(dest => dest.Iso31661NumCode, src => src.Iso31661NumCode)
              .Map(dest => dest.Domain, src => src.Domain)
              .Map(dest => dest.Currency, src => src.Currency)
              .Map(dest => dest.CurrencyCode, src => src.CurrencyCode)
              .Map(dest => dest.InternationalPhoneCode, src => src.InternationalPhoneCode);

        config.NewConfig<CountryResponse, Country>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Iso31661A2Code, src => src.Iso31661A2Code)
            .Map(dest => dest.Iso31661A3Code, src => src.Iso31661A3Code)
            .Map(dest => dest.Iso31661NumCode, src => src.Iso31661NumCode)
            .Map(dest => dest.Domain, src => src.Domain)
            .Map(dest => dest.Currency, src => src.Currency)
            .Map(dest => dest.CurrencyCode, src => src.CurrencyCode)
            .Map(dest => dest.InternationalPhoneCode, src => src.InternationalPhoneCode)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!);
    }
}

