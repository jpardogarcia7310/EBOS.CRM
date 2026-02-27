using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;

public class EndAccountHierarchyCommandValidator : AbstractValidator<EndAccountHierarchyCommand>
{
    public EndAccountHierarchyCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AccountHierarchyRequest).NotNull();
        RuleFor(x => x.AccountHierarchyRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.AccountHierarchyRequest.ValidTo).NotEmpty();
    }
}
