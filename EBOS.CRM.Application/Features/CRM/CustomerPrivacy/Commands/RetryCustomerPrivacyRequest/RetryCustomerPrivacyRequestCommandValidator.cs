using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;

public sealed class RetryCustomerPrivacyRequestCommandValidator : AbstractValidator<RetryCustomerPrivacyRequestCommand>
{
    public RetryCustomerPrivacyRequestCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Request).NotNull();
        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.TenantId).GreaterThan(0);
            RuleFor(x => x.Request.Reason).MaximumLength(1000);
        });
    }
}
