

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetCustomerAddressById;

public class GetCustomerAddressByIdQueryValidator : AbstractValidator<GetCustomerAddressByIdQuery>
{
    public GetCustomerAddressByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




