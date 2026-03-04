using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public sealed class MappingChannelCountry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ChannelCountry, ChannelCountryResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ChannelTypeId, src => src.ChannelTypeId)
            .Map(dest => dest.ChannelTypeDescripcion, src => src.ChannelType.Descripcion)
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.CountryIso2, src => src.Country.Iso31661A2Code)
            .Map(dest => dest.CountryName, src => src.Country.Name)
            .Map(dest => dest.CompositeKey, src => $"{src.ChannelTypeId}:{src.CountryId}")
            .Map(dest => dest.IsActive, src => src.IsActive);

        config.NewConfig<ChannelCountryResponse, ChannelCountry>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ChannelTypeId, src => src.ChannelTypeId)
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.UpdatedBy)
            .Ignore(dest => dest.ChannelType)
            .Ignore(dest => dest.Country);
    }
}
