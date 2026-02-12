using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;

public class GetCustomerConsentsByCustomerQueryValidator : AbstractValidator<GetCustomerConsentsByCustomerQuery>
{
    public GetCustomerConsentsByCustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
    }
}
