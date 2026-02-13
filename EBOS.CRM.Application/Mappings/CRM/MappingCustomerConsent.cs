using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCustomerConsent : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerConsent, CustomerConsentResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCustomerConsentRequest, CustomerConsent>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.ConsentType, src => src.ConsentType)
            .Map(dest => dest.Granted, src => src.Granted)
            .Map(dest => dest.GrantedAt, src => src.GrantedAt)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.ExpiresAt, src => src.ExpiresAt)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Customer);
    }
}
