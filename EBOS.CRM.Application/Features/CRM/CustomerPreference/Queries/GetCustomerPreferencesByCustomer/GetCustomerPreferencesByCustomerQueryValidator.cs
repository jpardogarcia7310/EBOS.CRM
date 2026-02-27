using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;

public class GetCustomerPreferencesByCustomerQueryValidator : AbstractValidator<GetCustomerPreferencesByCustomerQuery>
{
    public GetCustomerPreferencesByCustomerQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
