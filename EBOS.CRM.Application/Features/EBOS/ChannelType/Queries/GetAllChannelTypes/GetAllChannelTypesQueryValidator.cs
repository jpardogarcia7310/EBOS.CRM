using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;

public class GetAllChannelTypesQueryValidator : AbstractValidator<GetAllChannelTypesQuery>
{
    public GetAllChannelTypesQueryValidator()
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
