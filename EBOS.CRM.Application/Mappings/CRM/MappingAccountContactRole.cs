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
            .ConstructUsing(src => AccountContactRole.Create(
                src.TenantId,
                src.AccountContactId,
                src.RoleCode,
                src.IsPrimary,
                src.ValidFrom,
                src.ValidTo))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.RowVersion)
            .Ignore(dest => dest.AccountContact);

        config.NewConfig<UpdateAccountContactRoleRequest, AccountContactRole>()
            .ConstructUsing(src => AccountContactRole.Create(
                src.TenantId,
                src.AccountContactId,
                src.RoleCode,
                src.IsPrimary,
                src.ValidFrom,
                src.ValidTo))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.RowVersion)
            .Ignore(dest => dest.AccountContact);
    }
}
