using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;

public class GetCustomerPreferencesByCustomerQueryValidator : AbstractValidator<GetCustomerPreferencesByCustomerQuery>
{
    public GetCustomerPreferencesByCustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
    }
}
