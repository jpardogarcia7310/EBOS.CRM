using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;

public class AssignCaseQueueCommandValidator : AbstractValidator<AssignCaseQueueCommand>
{
    public AssignCaseQueueCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CaseRequest).NotNull();

        When(x => x.CaseRequest != null, () =>
        {
            RuleFor(x => x.CaseRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.CaseRequest.QueueId).GreaterThan(0);
        });
    }
}
