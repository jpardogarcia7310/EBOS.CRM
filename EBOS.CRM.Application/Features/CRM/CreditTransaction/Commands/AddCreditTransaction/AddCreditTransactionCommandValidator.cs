using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;

public class AddCreditTransactionCommandValidator : AbstractValidator<AddCreditTransactionCommand>
{
    public AddCreditTransactionCommandValidator()
    {
        RuleFor(x => x.CreditTransactionRequest).NotNull();
        RuleFor(x => x.CreditTransactionRequest.Type).NotEmpty(); RuleFor(x => x.CreditTransactionRequest.ExternalReference).NotEmpty(); RuleFor(x => x.CreditTransactionRequest.Comments).NotEmpty();

        RuleFor(x => x.CreditTransactionRequest.CreditAccountId).GreaterThan(0);
    }
}




