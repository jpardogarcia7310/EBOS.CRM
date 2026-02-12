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
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.CorporateCustomerId, src => src.CorporateCustomerId)
            .Map(dest => dest.IndividualCustomerId, src => src.IndividualCustomerId)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.StartAt, src => src.StartAt)
            .Map(dest => dest.EndAt, src => src.EndAt)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.CorporateCustomer)
            .Ignore(dest => dest.IndividualCustomer)
            .Ignore(dest => dest.Roles);

        config.NewConfig<UpdateAccountContactRequest, AccountContact>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.CorporateCustomerId, src => src.CorporateCustomerId)
            .Map(dest => dest.IndividualCustomerId, src => src.IndividualCustomerId)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.StartAt, src => src.StartAt)
            .Map(dest => dest.EndAt, src => src.EndAt)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.CorporateCustomer)
            .Ignore(dest => dest.IndividualCustomer)
            .Ignore(dest => dest.Roles);
    }
}
