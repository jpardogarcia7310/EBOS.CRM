using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingAccountHierarchy : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AccountHierarchy, AccountHierarchyResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddAccountHierarchyRequest, AccountHierarchy>()
            .ConstructUsing(src => AccountHierarchy.Create(
                src.TenantId,
                src.ParentCorporateCustomerId,
                src.ChildCorporateCustomerId,
                src.RelationType,
                src.ValidFrom))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.IsCurrent)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.RowVersion)
            .Ignore(dest => dest.ValidTo!)
            .Ignore(dest => dest.ParentCorporateCustomer)
            .Ignore(dest => dest.ChildCorporateCustomer);
    }
}
