using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;

public class GetValidationRuleByIdQueryValidator : AbstractValidator<GetValidationRuleByIdQuery>
{
    public GetValidationRuleByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}
