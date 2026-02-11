using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using global::EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCase : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Case, CaseResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Priority, src => src.Priority)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.QueueId, src => src.QueueId)
            .Map(dest => dest.SlaId, src => src.SlaId)
            .Map(dest => dest.DueAt, src => src.DueAt)
            .Map(dest => dest.ClosedAt, src => src.ClosedAt)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCaseRequest, Case>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Priority, src => src.Priority)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.QueueId, src => src.QueueId)
            .Map(dest => dest.SlaId, src => src.SlaId)
            .Map(dest => dest.DueAt, src => src.DueAt)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Map(dest => dest.UpdatedAt, src => (DateTime?)null)
            .Map(dest => dest.UpdatedBy, src => (long?)null)
            .Map(dest => dest.ClosedAt, src => (DateTime?)null)
            .Ignore(dest => dest.Activities)
            .Ignore(dest => dest.Queue)
            .Ignore(dest => dest.Sla);

        config.NewConfig<UpdateCaseRequest, Case>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Priority, src => src.Priority)
            .Map(dest => dest.OwnerUserId, src => src.OwnerUserId)
            .Map(dest => dest.QueueId, src => src.QueueId)
            .Map(dest => dest.SlaId, src => src.SlaId)
            .Map(dest => dest.DueAt, src => src.DueAt)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Map(dest => dest.UpdatedAt, src => (DateTime?)null)
            .Map(dest => dest.UpdatedBy, src => (long?)null)
            .Map(dest => dest.ClosedAt, src => (DateTime?)null)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Activities)
            .Ignore(dest => dest.Queue)
            .Ignore(dest => dest.Sla);
    }
}
