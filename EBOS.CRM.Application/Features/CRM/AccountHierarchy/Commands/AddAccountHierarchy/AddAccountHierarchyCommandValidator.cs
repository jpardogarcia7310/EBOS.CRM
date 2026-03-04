using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;

public class AddAccountHierarchyCommandValidator : AbstractValidator<AddAccountHierarchyCommand>
{
    public AddAccountHierarchyCommandValidator()
    {
        RuleFor(x => x.AccountHierarchyRequest).NotNull();
        RuleFor(x => x.AccountHierarchyRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.AccountHierarchyRequest.ParentCorporateCustomerId).GreaterThan(0);
        RuleFor(x => x.AccountHierarchyRequest.ChildCorporateCustomerId).GreaterThan(0);
        RuleFor(x => x.AccountHierarchyRequest.RelationType).NotEmpty();
        RuleFor(x => x.AccountHierarchyRequest.ValidFrom).NotEmpty();
        RuleFor(x => x.AccountHierarchyRequest)
            .Must(r => r.ParentCorporateCustomerId != r.ChildCorporateCustomerId)
            .WithMessage("ParentCorporateCustomerId cannot be the same as ChildCorporateCustomerId.");
    }
}
