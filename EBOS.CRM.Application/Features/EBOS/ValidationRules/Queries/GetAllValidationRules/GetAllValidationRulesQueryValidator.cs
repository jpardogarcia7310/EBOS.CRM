using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetAllValidationRules;

public class GetAllValidationRulesQueryValidator : AbstractValidator<GetAllValidationRulesQuery>
{
    public GetAllValidationRulesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0.")
            .WithErrorCode("VAL_PAGE_POSITIVE");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.")
            .WithErrorCode("VAL_SIZE_POSITIVE");
    }
}
