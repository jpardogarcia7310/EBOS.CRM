using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.CloseCase;

public class CloseCaseCommandValidator : AbstractValidator<CloseCaseCommand>
{
    public CloseCaseCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CaseRequest).NotNull();

        When(x => x.CaseRequest != null, () =>
        {
            RuleFor(x => x.CaseRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.CaseRequest.ClosedAt).NotEmpty();
        });
    }
}
