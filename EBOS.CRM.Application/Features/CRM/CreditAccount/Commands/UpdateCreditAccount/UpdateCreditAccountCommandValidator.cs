using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;

public class UpdateCreditAccountCommandValidator : AbstractValidator<UpdateCreditAccountCommand>
{
    public UpdateCreditAccountCommandValidator()
    {
        RuleFor(x => x.CreditAccountRequest).NotNull();
        RuleFor(x => x.CreditAccountRequest.Id).GreaterThan(0);

        RuleFor(x => x.CreditAccountRequest.MaxAmount).GreaterThan(0);
        RuleFor(x => x.CreditAccountRequest.UsedAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CreditAccountRequest)
            .Must(r => r.UsedAmount <= r.MaxAmount)
            .WithMessage("UsedAmount must be less than or equal to MaxAmount.");

        RuleFor(x => x.CreditAccountRequest.CustomerId).GreaterThan(0);
    }
}
