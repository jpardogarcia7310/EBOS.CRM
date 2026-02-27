using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingAccountContactRole : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AccountContactRole, AccountContactRoleResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddAccountContactRoleRequest, AccountContactRole>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.AccountContactId, src => src.AccountContactId)
            .Map(dest => dest.RoleCode, src => src.RoleCode)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.ValidFrom, src => src.ValidFrom)
            .Map(dest => dest.ValidTo, src => src.ValidTo)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.AccountContact);

        config.NewConfig<UpdateAccountContactRoleRequest, AccountContactRole>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.AccountContactId, src => src.AccountContactId)
            .Map(dest => dest.RoleCode, src => src.RoleCode)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.ValidFrom, src => src.ValidFrom)
            .Map(dest => dest.ValidTo, src => src.ValidTo)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.AccountContact);
    }
}
