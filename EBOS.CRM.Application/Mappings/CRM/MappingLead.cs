using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingLead : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Lead, LeadResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.CompanyName, src => src.CompanyName)
            .Map(dest => dest.ContactName, src => src.ContactName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.EstimatedValue, src => src.EstimatedValue)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.ConvertedOpportunityId, src => src.ConvertedOpportunityId)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddLeadRequest, Lead>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.CompanyName, src => src.CompanyName)
            .Map(dest => dest.ContactName, src => src.ContactName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.EstimatedValue, src => src.EstimatedValue)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.ConvertedOpportunityId!)
            .Ignore(dest => dest.ConvertedOpportunity!);

        config.NewConfig<UpdateLeadRequest, Lead>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.CompanyName, src => src.CompanyName)
            .Map(dest => dest.ContactName, src => src.ContactName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.EstimatedValue, src => src.EstimatedValue)
            .Map(dest => dest.Notes, src => src.Notes)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.ConvertedOpportunityId!)
            .Ignore(dest => dest.ConvertedOpportunity!);
    }
}
