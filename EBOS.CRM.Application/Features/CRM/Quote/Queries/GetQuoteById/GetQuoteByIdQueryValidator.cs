using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetQuoteById;

public class GetQuoteByIdQueryValidator : AbstractValidator<GetQuoteByIdQuery>
{
    public GetQuoteByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
