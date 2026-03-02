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
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.ParentCorporateCustomerId, src => src.ParentCorporateCustomerId)
            .Map(dest => dest.ChildCorporateCustomerId, src => src.ChildCorporateCustomerId)
            .Map(dest => dest.RelationType, src => src.RelationType)
            .Map(dest => dest.ValidFrom, src => src.ValidFrom)
            .Map(dest => dest.IsCurrent, _ => true)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.RowVersion)
            .Ignore(dest => dest.ValidTo!)
            .Ignore(dest => dest.ParentCorporateCustomer)
            .Ignore(dest => dest.ChildCorporateCustomer);
    }
}
