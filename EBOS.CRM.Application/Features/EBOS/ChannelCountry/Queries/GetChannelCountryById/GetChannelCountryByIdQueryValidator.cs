using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;

public class GetChannelCountryByIdQueryValidator : AbstractValidator<GetChannelCountryByIdQuery>
{
    public GetChannelCountryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}
