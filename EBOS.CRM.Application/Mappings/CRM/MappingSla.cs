using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingSla : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Sla, SlaResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.TargetMinutes, src => src.TargetMinutes)
            .Map(dest => dest.WarningMinutes, src => src.WarningMinutes)
            .Map(dest => dest.ActiveFrom, src => src.ActiveFrom)
            .Map(dest => dest.ActiveTo, src => src.ActiveTo)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddSlaRequest, Sla>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.TargetMinutes, src => src.TargetMinutes)
            .Map(dest => dest.WarningMinutes, src => src.WarningMinutes)
            .Map(dest => dest.ActiveFrom, src => src.ActiveFrom)
            .Map(dest => dest.ActiveTo, src => src.ActiveTo)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Cases);

        config.NewConfig<UpdateSlaRequest, Sla>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.TargetMinutes, src => src.TargetMinutes)
            .Map(dest => dest.WarningMinutes, src => src.WarningMinutes)
            .Map(dest => dest.ActiveFrom, src => src.ActiveFrom)
            .Map(dest => dest.ActiveTo, src => src.ActiveTo)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Cases);
    }
}
