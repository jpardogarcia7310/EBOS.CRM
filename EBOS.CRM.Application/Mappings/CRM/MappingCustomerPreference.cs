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
            .ConstructUsing(src => CustomerPreference.Create(
                src.TenantId,
                src.CustomerId,
                src.ChannelId,
                src.Preferred,
                DateTime.UtcNow,
                1))
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.UpdatedBy)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.Channel)
            .Ignore(dest => dest.RowVersion);
    }
}
