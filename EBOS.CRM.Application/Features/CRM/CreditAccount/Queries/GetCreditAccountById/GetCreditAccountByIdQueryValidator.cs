using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;

public class GetCreditAccountByIdQueryValidator : AbstractValidator<GetCreditAccountByIdQuery>
{
    public GetCreditAccountByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




