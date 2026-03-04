using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingAccountContact : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AccountContact, AccountContactResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddAccountContactRequest, AccountContact>()
            .ConstructUsing(src => AccountContact.Create(
                src.TenantId,
                src.CorporateCustomerId,
                src.IndividualCustomerId,
                src.IsPrimary,
                src.StartAt,
                src.EndAt,
                1,
                null))
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.RowVersion)
            .Ignore(dest => dest.CorporateCustomer)
            .Ignore(dest => dest.IndividualCustomer)
            .Ignore(dest => dest.Roles);

        config.NewConfig<UpdateAccountContactRequest, AccountContact>()
            .ConstructUsing(src => AccountContact.Create(
                src.TenantId,
                src.CorporateCustomerId,
                src.IndividualCustomerId,
                src.IsPrimary,
                src.StartAt,
                src.EndAt,
                1,
                null))
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.RowVersion)
            .Ignore(dest => dest.CorporateCustomer)
            .Ignore(dest => dest.IndividualCustomer)
            .Ignore(dest => dest.Roles);
    }
}
