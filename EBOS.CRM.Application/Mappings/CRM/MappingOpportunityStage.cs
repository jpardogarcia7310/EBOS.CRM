using EBOS.CRM.Application.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingOpportunityStage : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OpportunityStage, OpportunityStageResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Order, src => src.Order)
            .Map(dest => dest.DefaultProbability, src => src.DefaultProbability)
            .Map(dest => dest.IsClosed, src => src.IsClosed)
            .Map(dest => dest.IsWon, src => src.IsWon)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddOpportunityStageRequest, OpportunityStage>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Order, src => src.Order)
            .Map(dest => dest.DefaultProbability, src => src.DefaultProbability)
            .Map(dest => dest.IsClosed, src => src.IsClosed)
            .Map(dest => dest.IsWon, src => src.IsWon)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Opportunities);

        config.NewConfig<UpdateOpportunityStageRequest, OpportunityStage>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Order, src => src.Order)
            .Map(dest => dest.DefaultProbability, src => src.DefaultProbability)
            .Map(dest => dest.IsClosed, src => src.IsClosed)
            .Map(dest => dest.IsWon, src => src.IsWon)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Opportunities);
    }
}
