using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using global::EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCustomerAddress : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerAddress, CustomerAddressResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCustomerAddressRequest, CustomerAddress>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
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
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.Address);

        config.NewConfig<UpdateCustomerAddressRequest, CustomerAddress>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
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
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.Address);
    }
}


