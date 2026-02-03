

using FluentValidation;


namespace EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;

public class GetAddressTypeByIdQueryValidator : AbstractValidator<GetAddressTypeByIdQuery>
{
    public GetAddressTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("The identifier must be a positive integer greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}



