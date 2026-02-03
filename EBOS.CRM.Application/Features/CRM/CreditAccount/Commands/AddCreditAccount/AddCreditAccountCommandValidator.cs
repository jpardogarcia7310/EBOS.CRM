

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;

public class AddCreditAccountCommandValidator : AbstractValidator<AddCreditAccountCommand>
{
    public AddCreditAccountCommandValidator()
    {
        RuleFor(x => x.CreditAccountRequest).NotNull();


        RuleFor(x => x.CreditAccountRequest.CustomerId).GreaterThan(0);
    }
}




