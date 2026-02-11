using FluentValidation;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;

public class UpdateCaseCommandValidator : AbstractValidator<UpdateCaseCommand>
{
    public UpdateCaseCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CaseRequest).NotNull();

        When(x => x.CaseRequest != null, () =>
        {
            RuleFor(x => x.CaseRequest.Id).GreaterThan(0);
            RuleFor(x => x.CaseRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.CaseRequest.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.CaseRequest.Description).MaximumLength(2000);
            RuleFor(x => x.CaseRequest.Status).NotEmpty().MaximumLength(50)
                .Must(CaseEntity.IsValidStatus);
            RuleFor(x => x.CaseRequest.Priority).NotEmpty().MaximumLength(50).
                Must(CaseEntity.IsValidPriority);
            RuleFor(x => x.CaseRequest.OwnerUserId).GreaterThan(0);
            RuleFor(x => x.CaseRequest.QueueId).GreaterThan(0);
            RuleFor(x => x.CaseRequest.SlaId).GreaterThan(0);
        });
    }
}
