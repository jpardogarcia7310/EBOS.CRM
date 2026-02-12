using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.RouteCase;

public sealed class RouteCaseCommandValidator : AbstractValidator<RouteCaseCommand>
{
    public RouteCaseCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CaseRequest).NotNull();

        When(x => x.CaseRequest != null, () =>
        {
            RuleFor(x => x.CaseRequest.TenantId).GreaterThan(0);
        });
    }
}
