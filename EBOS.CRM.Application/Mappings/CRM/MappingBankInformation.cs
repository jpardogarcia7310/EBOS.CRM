using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using global::EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingBankInformation : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BankInformation, BankInformationResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddBankInformationRequest, BankInformation>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Iban, src => src.Iban)
            .Map(dest => dest.Bic, src => src.Bic)
            .Map(dest => dest.BankName, src => src.BankName)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Customer);

        config.NewConfig<UpdateBankInformationRequest, BankInformation>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Iban, src => src.Iban)
            .Map(dest => dest.Bic, src => src.Bic)
            .Map(dest => dest.BankName, src => src.BankName)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Customer);
    }
}


