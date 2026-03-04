using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public class MappingTenantQuota : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TenantQuota, TenantQuotaResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Metric, src => src.Metric)
            .Map(dest => dest.Limit, src => src.Limit)
            .Map(dest => dest.Unit, src => src.Unit)
            .Map(dest => dest.EffectiveFrom, src => src.EffectiveFrom)
            .Map(dest => dest.EffectiveTo, src => src.EffectiveTo)
            .Map(dest => dest.Active, src => !src.Erased);
    }
}
