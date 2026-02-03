

using FluentValidation;


namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public class GetIdentificationTypeByIdQueryValidator : AbstractValidator<GetIdentificationTypeByIdQuery>
{
    public GetIdentificationTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("The identifier must be a positive integer greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}




