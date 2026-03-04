using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;

public class GetCustomerMergeHistoryByWinnerQueryValidator : AbstractValidator<GetCustomerMergeHistoryByWinnerQuery>
{
    public GetCustomerMergeHistoryByWinnerQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.WinnerCustomerId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
