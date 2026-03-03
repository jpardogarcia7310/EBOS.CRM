using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;

public sealed class GetCustomerPrivacyRequestsByCustomerQueryValidator
    : AbstractValidator<GetCustomerPrivacyRequestsByCustomerQuery>
{
    public GetCustomerPrivacyRequestsByCustomerQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
