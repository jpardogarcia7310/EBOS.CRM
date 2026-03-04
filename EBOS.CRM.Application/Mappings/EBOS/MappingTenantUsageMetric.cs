using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public class MappingTenantUsageMetric : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TenantUsageMetric, TenantUsageMetricResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Metric, src => src.Metric)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Unit, src => src.Unit)
            .Map(dest => dest.PeriodStart, src => src.PeriodStart)
            .Map(dest => dest.PeriodEnd, src => src.PeriodEnd)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.Active, src => !src.Erased);
    }
}
