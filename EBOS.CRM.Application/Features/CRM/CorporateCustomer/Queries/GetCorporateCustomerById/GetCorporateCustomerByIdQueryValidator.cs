using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;

public class GetCorporateCustomerByIdQueryValidator : AbstractValidator<GetCorporateCustomerByIdQuery>
{
    public GetCorporateCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




