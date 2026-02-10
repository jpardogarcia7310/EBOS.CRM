using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;

public class AddQuoteCommandValidator : AbstractValidator<AddQuoteCommand>
{
    public AddQuoteCommandValidator()
    {
        RuleFor(x => x.QuoteRequest).NotNull();

        When(x => x.QuoteRequest != null, () =>
        {
            RuleFor(x => x.QuoteRequest.OpportunityId).GreaterThan(0);
            RuleFor(x => x.QuoteRequest.Status).NotEmpty().MaximumLength(50);
            RuleFor(x => x.QuoteRequest.ReferenceNumber).MaximumLength(50);
            RuleFor(x => x.QuoteRequest.SubtotalAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuoteRequest.DiscountAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuoteRequest.TotalAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuoteRequest.Notes).MaximumLength(2000);
            RuleFor(x => x.QuoteRequest.Status).NotEmpty().MaximumLength(50);
            RuleFor(x => x.QuoteRequest.DiscountAmount).LessThanOrEqualTo(x => x.QuoteRequest.SubtotalAmount);
        });
    }
}
