using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;

public class RevokeCustomerConsentCommandValidator : AbstractValidator<RevokeCustomerConsentCommand>
{
    public RevokeCustomerConsentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ConsentRequest).NotNull();
        RuleFor(x => x.ConsentRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.ConsentRequest.RevokedAt).NotEmpty();
    }
}
