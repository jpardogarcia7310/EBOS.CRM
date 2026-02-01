using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;

public class GetIndividualCustomerByIdQueryValidator : AbstractValidator<GetIndividualCustomerByIdQuery>
{
    public GetIndividualCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
