using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandValidator : AbstractValidator<AddCustomerConsentCommand>
{
    public AddCustomerConsentCommandValidator()
    {
        RuleFor(x => x.ConsentRequest).NotNull();
        RuleFor(x => x.ConsentRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.ConsentRequest.CustomerId).GreaterThan(0);
        RuleFor(x => x.ConsentRequest.ConsentType).NotEmpty();
        RuleFor(x => x.ConsentRequest.Source).NotEmpty();
        RuleFor(x => x.ConsentRequest.GrantedAt).NotEmpty();
    }
}
