using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Entities.CRM;
using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;

public sealed class RegisterCustomerPrivacyRequestCommandValidator : AbstractValidator<RegisterCustomerPrivacyRequestCommand>
{
    public RegisterCustomerPrivacyRequestCommandValidator()
    {
        RuleFor(x => x.Request).NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.TenantId).GreaterThan(0);
            RuleFor(x => x.Request.CustomerId).GreaterThan(0);
            RuleFor(x => x.Request.RequestType)
                .NotEmpty()
                .Must(BeValidRequestType)
                .WithMessage("RequestType is invalid.");
            RuleFor(x => x.Request.Reason)
                .MaximumLength(1000);
        });
    }

    private static bool BeValidRequestType(string requestType)
    {
        var normalized = requestType.Trim().ToUpperInvariant();
        return normalized is CustomerPrivacyRequest.TypeForget
            or CustomerPrivacyRequest.TypeAnonymize
            or CustomerPrivacyRequest.TypeRetentionReview;
    }
}
