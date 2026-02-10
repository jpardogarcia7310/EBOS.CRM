using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public class GetAllQuotesQueryValidator : AbstractValidator<GetAllQuotesQuery>
{
    public GetAllQuotesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
