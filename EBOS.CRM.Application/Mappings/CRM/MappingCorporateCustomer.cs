using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCorporateCustomer : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CorporateCustomer, CorporateCustomerResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Map(dest => dest.LegalName, src => src.LegalName)
            .Map(dest => dest.TaxIdentification, src => src.TaxIdentification)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCorporateCustomerRequest, CorporateCustomer>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Map(dest => dest.LegalName, src => src.LegalName)
            .Map(dest => dest.TaxIdentification, src => src.TaxIdentification)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.CreditAccount!)
            .Ignore(dest => dest.TaxInformation!)
            .Ignore(dest => dest.BankInformation!)
            .Ignore(dest => dest.Addresses)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.BranchOffices);

        config.NewConfig<UpdateCorporateCustomerRequest, CorporateCustomer>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Map(dest => dest.LegalName, src => src.LegalName)
            .Map(dest => dest.TaxIdentification, src => src.TaxIdentification)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.CreditAccount!)
            .Ignore(dest => dest.TaxInformation!)
            .Ignore(dest => dest.BankInformation!)
            .Ignore(dest => dest.Addresses)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.BranchOffices);
    }
}


