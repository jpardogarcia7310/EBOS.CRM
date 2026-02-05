using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;

public class UpdateCreditAccountCommandValidator : AbstractValidator<UpdateCreditAccountCommand>
{
    public UpdateCreditAccountCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CreditAccountRequest).NotNull();
        RuleFor(x => x.CreditAccountRequest.CustomerId).GreaterThan(0);
    }
}




