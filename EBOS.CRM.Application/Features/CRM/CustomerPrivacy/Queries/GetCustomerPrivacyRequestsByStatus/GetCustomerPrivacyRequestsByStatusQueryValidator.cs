using EBOS.CRM.Domain.Entities.CRM;
using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByStatus;

public sealed class GetCustomerPrivacyRequestsByStatusQueryValidator
    : AbstractValidator<GetCustomerPrivacyRequestsByStatusQuery>
{
    public GetCustomerPrivacyRequestsByStatusQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(BeValidStatus)
            .WithMessage("Status is invalid.");
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }

    private static bool BeValidStatus(string status)
    {
        var normalized = status.Trim().ToUpperInvariant();
        return normalized is CustomerPrivacyRequest.StatusPending
            or CustomerPrivacyRequest.StatusInProgress
            or CustomerPrivacyRequest.StatusCompleted
            or CustomerPrivacyRequest.StatusFailed
            or CustomerPrivacyRequest.StatusCanceled;
    }
}
