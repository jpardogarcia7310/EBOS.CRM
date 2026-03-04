using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;

public sealed class ExecuteCustomerPrivacyRequestCommandValidator : AbstractValidator<ExecuteCustomerPrivacyRequestCommand>
{
    public ExecuteCustomerPrivacyRequestCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Request).NotNull();
        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.TenantId).GreaterThan(0);
        });
    }
}
