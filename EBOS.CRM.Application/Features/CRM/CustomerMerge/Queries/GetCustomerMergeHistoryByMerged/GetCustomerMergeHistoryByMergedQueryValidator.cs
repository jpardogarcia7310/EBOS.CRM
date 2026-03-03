using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;

public class GetCustomerMergeHistoryByMergedQueryValidator : AbstractValidator<GetCustomerMergeHistoryByMergedQuery>
{
    public GetCustomerMergeHistoryByMergedQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.MergedCustomerId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
