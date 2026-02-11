using FluentValidation;
using CaseActivityEntity = EBOS.CRM.Domain.Entities.CRM.CaseActivity;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;

public class UpdateCaseActivityCommandValidator : AbstractValidator<UpdateCaseActivityCommand>
{
    public UpdateCaseActivityCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ActivityRequest).NotNull();

        When(x => x.ActivityRequest != null, () =>
        {
            RuleFor(x => x.ActivityRequest.Id).GreaterThan(0);
            RuleFor(x => x.ActivityRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.ActivityRequest.CaseId).GreaterThan(0);
            RuleFor(x => x.ActivityRequest.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ActivityRequest.Description).MaximumLength(2000);
            RuleFor(x => x.ActivityRequest.Status)
                .NotEmpty().MaximumLength(50)
                .Must(status => status is CaseActivityEntity.StatusOpen
                    or CaseActivityEntity.StatusInProgress
                    or CaseActivityEntity.StatusCompleted
                    or CaseActivityEntity.StatusCancelled);
        });
    }
}
