using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public class MappingTenantConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TenantConfiguration, TenantConfigurationResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Key, src => src.Key)
            .Map(dest => dest.ValueJson, src => src.ValueJson)
            .Map(dest => dest.Active, src => !src.Erased);
    }
}
