using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.UpdateCreditTransaction;

public class UpdateCreditTransactionCommandValidator : AbstractValidator<UpdateCreditTransactionCommand>
{
    public UpdateCreditTransactionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CreditTransactionRequest).NotNull();
        RuleFor(x => x.CreditTransactionRequest.Type).NotEmpty();
        RuleFor(x => x.CreditTransactionRequest.ExternalReference).NotEmpty();
        RuleFor(x => x.CreditTransactionRequest.Comments).NotEmpty();
        RuleFor(x => x.CreditTransactionRequest.CreditAccountId).GreaterThan(0);
    }
}




