using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;

public class GetAccountHierarchyByIdQueryValidator : AbstractValidator<GetAccountHierarchyByIdQuery>
{
    public GetAccountHierarchyByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
