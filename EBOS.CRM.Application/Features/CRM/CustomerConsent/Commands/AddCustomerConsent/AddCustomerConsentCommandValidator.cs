using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandValidator : AbstractValidator<AddCustomerConsentCommand>
{
    public AddCustomerConsentCommandValidator()
    {
        RuleFor(x => x.ConsentRequest).NotNull();
        RuleFor(x => x.ConsentRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.ConsentRequest.CustomerId).GreaterThan(0);
        RuleFor(x => x.ConsentRequest.ConsentType)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.ConsentRequest.Source)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.ConsentRequest.GrantedAt).NotEmpty();
        RuleFor(x => x.ConsentRequest.ExpiresAt)
            .Must((request, expiresAt) => !expiresAt.HasValue || expiresAt.Value >= request.ConsentRequest.GrantedAt)
            .WithMessage("ExpiresAt cannot be earlier than GrantedAt.");

        When(x => !x.ConsentRequest.Granted, () =>
        {
            RuleFor(x => x.ConsentRequest.ExpiresAt)
                .NotNull()
                .WithMessage("ExpiresAt is required for expire events.")
                .Must((request, expiresAt) => expiresAt.HasValue && expiresAt.Value == request.ConsentRequest.GrantedAt)
                .WithMessage("ExpiresAt must match GrantedAt for expire events.");
        });
    }
}
