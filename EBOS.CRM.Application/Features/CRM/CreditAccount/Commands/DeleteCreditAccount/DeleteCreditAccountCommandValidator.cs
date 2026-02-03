using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;

public class DeleteCreditAccountCommandValidator : AbstractValidator<DeleteCreditAccountCommand>
{
    public DeleteCreditAccountCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




