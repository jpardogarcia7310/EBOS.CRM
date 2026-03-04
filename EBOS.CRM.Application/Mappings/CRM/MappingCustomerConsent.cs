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

    }
}
