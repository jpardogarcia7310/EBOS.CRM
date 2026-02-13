using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCustomerPreference : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerPreference, CustomerPreferenceResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<UpsertCustomerPreferenceRequest, CustomerPreference>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.ChannelId, src => src.ChannelId)
            .Map(dest => dest.Preferred, src => src.Preferred)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.UpdatedBy)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.Channel);
    }
}
