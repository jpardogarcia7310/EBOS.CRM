using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;

public class AssignCaseSlaCommandValidator : AbstractValidator<AssignCaseSlaCommand>
{
    public AssignCaseSlaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CaseRequest).NotNull();

        When(x => x.CaseRequest != null, () =>
        {
            RuleFor(x => x.CaseRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.CaseRequest.SlaId).GreaterThan(0);
        });
    }
}
