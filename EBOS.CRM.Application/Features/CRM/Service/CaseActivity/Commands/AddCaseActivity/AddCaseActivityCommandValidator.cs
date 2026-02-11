using EBOS.CRM.Domain.Entities.CRM;
using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;

public class AddCaseActivityCommandValidator : AbstractValidator<AddCaseActivityCommand>
{
    public AddCaseActivityCommandValidator()
    {
        RuleFor(x => x.ActivityRequest).NotNull();

        When(x => x.ActivityRequest != null, () =>
        {
            RuleFor(x => x.ActivityRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.ActivityRequest.CaseId).GreaterThan(0);
            RuleFor(x => x.ActivityRequest.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ActivityRequest.Description).MaximumLength(2000);
            RuleFor(x => x.ActivityRequest.Status)
                .NotEmpty().MaximumLength(50)
                .Must(status => status is CaseActivity.StatusOpen
                    or CaseActivity.StatusInProgress
                    or CaseActivity.StatusCompleted
                    or CaseActivity.StatusCancelled);
        });
    }
}
