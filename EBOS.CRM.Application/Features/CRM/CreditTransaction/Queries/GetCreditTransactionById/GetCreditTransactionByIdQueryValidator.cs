using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;

public class GetCreditTransactionByIdQueryValidator : AbstractValidator<GetCreditTransactionByIdQuery>
{
    public GetCreditTransactionByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




