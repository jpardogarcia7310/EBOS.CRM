using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;

public class PatchCreditAccountCommandValidator : AbstractValidator<PatchCreditAccountCommand>
{
    public PatchCreditAccountCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CreditAccountRequest).NotNull();

        RuleFor(x => x.CreditAccountRequest)
            .Must(r =>
                r.MaxAmount.HasValue ||
                r.UsedAmount.HasValue ||
                r.CustomerId.HasValue)
            .WithMessage("At least one field must be provided.");

        When(x => x.CreditAccountRequest.MaxAmount.HasValue, () =>
        {
            RuleFor(x => x.CreditAccountRequest.MaxAmount!.Value).GreaterThan(0);
        });

        When(x => x.CreditAccountRequest.UsedAmount.HasValue, () =>
        {
            RuleFor(x => x.CreditAccountRequest.UsedAmount!.Value).GreaterThanOrEqualTo(0);
        });

        When(x => x.CreditAccountRequest.CustomerId.HasValue, () =>
        {
            RuleFor(x => x.CreditAccountRequest.CustomerId!.Value).GreaterThan(0);
        });

        RuleFor(x => x.CreditAccountRequest)
            .Must(r =>
            {
                var max = r.MaxAmount;
                var used = r.UsedAmount;
                if (!max.HasValue || !used.HasValue)
                    return true;
                return used.Value <= max.Value;
            })
            .WithMessage("UsedAmount must be less than or equal to MaxAmount when both are provided.");
    }
}




