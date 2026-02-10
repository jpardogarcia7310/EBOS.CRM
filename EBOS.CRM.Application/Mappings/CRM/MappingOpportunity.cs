using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingOpportunity : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Opportunity, OpportunityResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.StageId, src => src.StageId)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.ExpectedCloseDate, src => src.ExpectedCloseDate)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Probability, src => src.Probability)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.SourceLeadId, src => src.SourceLeadId)
            .Map(dest => dest.CloseReason, src => src.CloseReason)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddOpportunityRequest, Opportunity>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.StageId, src => src.StageId)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.ExpectedCloseDate, src => src.ExpectedCloseDate)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Probability, src => src.Probability)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.SourceLeadId, src => src.SourceLeadId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.CloseReason!)
            .Ignore(dest => dest.Stage)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.SourceLead!)
            .Ignore(dest => dest.Quotes);

        config.NewConfig<UpdateOpportunityRequest, Opportunity>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.StageId, src => src.StageId)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.ExpectedCloseDate, src => src.ExpectedCloseDate)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Probability, src => src.Probability)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.SourceLeadId, src => src.SourceLeadId)
            .Map(dest => dest.CloseReason, src => src.CloseReason)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Stage)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.SourceLead!)
            .Ignore(dest => dest.Quotes);
    }
}
