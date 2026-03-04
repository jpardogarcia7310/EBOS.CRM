using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCustomer : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Customer, CustomerResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCustomerRequest, Customer>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.CreditAccount!)
            .Ignore(dest => dest.TaxInformation!)
            .Ignore(dest => dest.BankInformation!)
            .Ignore(dest => dest.Addresses)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.AccountContacts)
            .Ignore(dest => dest.Preferences)
            .Ignore(dest => dest.Consents)
            .Ignore(dest => dest.Source!)
            .Ignore(dest => dest.Confidentiality!);

        config.NewConfig<UpdateCustomerRequest, Customer>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.CreditAccount!)
            .Ignore(dest => dest.TaxInformation!)
            .Ignore(dest => dest.BankInformation!)
            .Ignore(dest => dest.Addresses)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.AccountContacts)
            .Ignore(dest => dest.Preferences)
            .Ignore(dest => dest.Consents)
            .Ignore(dest => dest.Source!)
            .Ignore(dest => dest.Confidentiality!);
    }
}


