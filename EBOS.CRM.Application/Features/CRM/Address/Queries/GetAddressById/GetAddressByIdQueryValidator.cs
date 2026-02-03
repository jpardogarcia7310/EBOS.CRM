

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;

public class GetAddressByIdQueryValidator : AbstractValidator<GetAddressByIdQuery>
{
    public GetAddressByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("The identifier must be a positive integer greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}



