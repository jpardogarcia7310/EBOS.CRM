using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingTaxInformationAddress : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaxInformationAddress, TaxInformationAddressResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddTaxInformationAddressRequest, TaxInformationAddress>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.TaxInformationId, src => src.TaxInformationId)
            .Map(dest => dest.AddressId, src => src.AddressId)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.ValidFrom, src => src.ValidFrom)
            .Map(dest => dest.ValidTo, src => src.ValidTo)
            .Map(dest => dest.IsCurrent, src => src.IsCurrent)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.TaxInformation)
            .Ignore(dest => dest.Address);

        config.NewConfig<UpdateTaxInformationAddressRequest, TaxInformationAddress>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.TaxInformationId, src => src.TaxInformationId)
            .Map(dest => dest.AddressId, src => src.AddressId)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.ValidFrom, src => src.ValidFrom)
            .Map(dest => dest.ValidTo, src => src.ValidTo)
            .Map(dest => dest.IsCurrent, src => src.IsCurrent)
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
            .Ignore(dest => dest.TaxInformation)
            .Ignore(dest => dest.Address);
    }
}


