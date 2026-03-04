using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.DeleteQuote;

public class DeleteQuoteCommandValidator : AbstractValidator<DeleteQuoteCommand>
{
    public DeleteQuoteCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
