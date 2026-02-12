using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public class UpsertCustomerPreferenceCommandValidator : AbstractValidator<UpsertCustomerPreferenceCommand>
{
    public UpsertCustomerPreferenceCommandValidator()
    {
        RuleFor(x => x.PreferenceRequest).NotNull();
        RuleFor(x => x.PreferenceRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.PreferenceRequest.CustomerId).GreaterThan(0);
        RuleFor(x => x.PreferenceRequest.ChannelId).GreaterThan(0);
    }
}
