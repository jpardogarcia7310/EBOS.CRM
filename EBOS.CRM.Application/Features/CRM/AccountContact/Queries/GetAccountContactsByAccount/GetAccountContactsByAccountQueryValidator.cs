using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;

public class GetAccountContactsByAccountQueryValidator : AbstractValidator<GetAccountContactsByAccountQuery>
{
    public GetAccountContactsByAccountQueryValidator()
    {
        RuleFor(x => x.CorporateCustomerId).GreaterThan(0);
    }
}
