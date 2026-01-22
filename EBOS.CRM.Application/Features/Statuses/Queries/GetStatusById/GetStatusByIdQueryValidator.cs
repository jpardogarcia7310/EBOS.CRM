using FluentValidation;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryValidator : AbstractValidator<GetStatusByIdQuery>
{
    public GetStatusByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("The identifier must be a positive integer greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}