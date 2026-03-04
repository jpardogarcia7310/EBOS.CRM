using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingQueue : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Queue, QueueResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.DefaultOwnerUserId, src => src.DefaultOwnerUserId)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddQueueRequest, Queue>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.DefaultOwnerUserId, src => src.DefaultOwnerUserId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Cases);

        config.NewConfig<UpdateQueueRequest, Queue>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.DefaultOwnerUserId, src => src.DefaultOwnerUserId)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Cases);
    }
}
