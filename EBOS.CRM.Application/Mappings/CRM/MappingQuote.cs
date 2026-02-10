using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingQuote : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Quote, QuoteResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.OpportunityId, src => src.OpportunityId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ReferenceNumber, src => src.ReferenceNumber)
            .Map(dest => dest.SubtotalAmount, src => src.SubtotalAmount)
            .Map(dest => dest.DiscountAmount, src => src.DiscountAmount)
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.ValidUntil, src => src.ValidUntil)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddQuoteRequest, Quote>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.OpportunityId, src => src.OpportunityId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ReferenceNumber, src => src.ReferenceNumber)
            .Map(dest => dest.SubtotalAmount, src => src.SubtotalAmount)
            .Map(dest => dest.DiscountAmount, src => src.DiscountAmount)
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.ValidUntil, src => src.ValidUntil)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.ValidUntil!)
            .Ignore(dest => dest.Opportunity);

        config.NewConfig<UpdateQuoteRequest, Quote>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.OpportunityId, src => src.OpportunityId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ReferenceNumber, src => src.ReferenceNumber)
            .Map(dest => dest.SubtotalAmount, src => src.SubtotalAmount)
            .Map(dest => dest.DiscountAmount, src => src.DiscountAmount)
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.ValidUntil, src => src.ValidUntil)
            .Map(dest => dest.Notes, src => src.Notes)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.ValidUntil!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Opportunity);
    }
}
