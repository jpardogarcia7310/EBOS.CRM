using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;

public class GetChannelTypeByIdQueryValidator : AbstractValidator<GetChannelTypeByIdQuery>
{
    public GetChannelTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("The identifier must be a positive integer greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}
