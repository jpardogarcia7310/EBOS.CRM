using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;

public class GetAccountHierarchyByAccountQueryValidator : AbstractValidator<GetAccountHierarchyByAccountQuery>
{
    public GetAccountHierarchyByAccountQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.CorporateCustomerId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
