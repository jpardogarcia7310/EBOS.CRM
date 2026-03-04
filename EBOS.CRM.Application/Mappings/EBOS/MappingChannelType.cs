using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public sealed class MappingChannelType : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ChannelType, ChannelTypeResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Descripcion, src => src.Descripcion)
            .Map(dest => dest.IsActive, src => src.IsActive);

        config.NewConfig<ChannelTypeResponse, ChannelType>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Descripcion, src => src.Descripcion)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.UpdatedBy)
            .Ignore(dest => dest.CustomerPreferences)
            .Ignore(dest => dest.ChannelCountries);
    }
}
