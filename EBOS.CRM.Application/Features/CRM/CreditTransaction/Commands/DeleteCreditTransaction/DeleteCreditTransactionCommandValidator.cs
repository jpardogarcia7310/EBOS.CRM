

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.DeleteCreditTransaction;

public class DeleteCreditTransactionCommandValidator : AbstractValidator<DeleteCreditTransactionCommand>
{
    public DeleteCreditTransactionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




