using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingTaxInformation : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaxInformation, TaxInformationResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddTaxInformationRequest, TaxInformation>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.TaxName, src => src.TaxName)
            .Map(dest => dest.TaxIdentificationNumber, src => src.TaxIdentificationNumber)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.TaxInformationAddresses);

        config.NewConfig<UpdateTaxInformationRequest, TaxInformation>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.TaxName, src => src.TaxName)
            .Map(dest => dest.TaxIdentificationNumber, src => src.TaxIdentificationNumber)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.TaxInformationAddresses);
    }
}


