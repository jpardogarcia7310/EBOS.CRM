using EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;
using FluentValidation;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeById;

public class GetIdentificationTypeQueryValidator : AbstractValidator<GetAddressTypeByIdQuery>
{
    public GetIdentificationTypeQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("The identifier must be a positive integer greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}