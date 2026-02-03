using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingIndividualCustomer : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IndividualCustomer, IndividualCustomerResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddIndividualCustomerRequest, IndividualCustomer>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.IdentificationNumber, src => src.IdentificationNumber)
            .Map(dest => dest.IdentificationTypeId, src => src.IdentificationTypeId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.CreditAccount!)
            .Ignore(dest => dest.TaxInformation!)
            .Ignore(dest => dest.BankInformation!)
            .Ignore(dest => dest.Addresses)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.IdentificationType);

        config.NewConfig<UpdateIndividualCustomerRequest, IndividualCustomer>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.StatusId, src => src.StatusId)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.IdentificationNumber, src => src.IdentificationNumber)
            .Map(dest => dest.IdentificationTypeId, src => src.IdentificationTypeId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.CreditAccount!)
            .Ignore(dest => dest.TaxInformation!)
            .Ignore(dest => dest.BankInformation!)
            .Ignore(dest => dest.Addresses)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.IdentificationType);
    }
}


