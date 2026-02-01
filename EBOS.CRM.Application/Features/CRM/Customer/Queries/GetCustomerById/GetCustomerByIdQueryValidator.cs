using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetCustomerById;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
