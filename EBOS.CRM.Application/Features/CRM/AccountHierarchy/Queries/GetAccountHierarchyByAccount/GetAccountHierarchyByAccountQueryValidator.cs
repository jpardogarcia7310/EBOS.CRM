using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;

public class GetAccountHierarchyByAccountQueryValidator : AbstractValidator<GetAccountHierarchyByAccountQuery>
{
    public GetAccountHierarchyByAccountQueryValidator()
    {
        RuleFor(x => x.CorporateCustomerId).GreaterThan(0);
    }
}
