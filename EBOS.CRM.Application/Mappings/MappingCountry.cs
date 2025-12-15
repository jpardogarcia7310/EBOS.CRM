using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingCountry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Country, CountryResponseDto>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.Iso31661A2Code, src => src.Iso31661A2Code)
              .Map(dest => dest.Iso31661A3Code, src => src.Iso31661A3Code)
              .Map(dest => dest.Iso31661NumCode, src => src.Iso31661NumCode)
              .Map(dest => dest.Domain, src => src.Domain)
              .Map(dest => dest.Currency, src => src.Currency)
              .Map(dest => dest.CurrencyCode, src => src.CurrencyCode)
              .Map(dest => dest.InternationalPhoneCode, src => src.InternationalPhoneCode);

        config.NewConfig<CountryResponseDto, Country>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.Iso31661A2Code, src => src.Iso31661A2Code)
              .Map(dest => dest.Iso31661A3Code, src => src.Iso31661A3Code)
              .Map(dest => dest.Iso31661NumCode, src => src.Iso31661NumCode)
              .Map(dest => dest.Domain, src => src.Domain)
              .Map(dest => dest.Currency, src => src.Currency)
              .Map(dest => dest.CurrencyCode, src => src.CurrencyCode)
              .Map(dest => dest.InternationalPhoneCode, src => src.InternationalPhoneCode)
              .Ignore(dest => dest.TaxAddresses)
              .Ignore(dest => dest.ShippingAddresses);
    }
}