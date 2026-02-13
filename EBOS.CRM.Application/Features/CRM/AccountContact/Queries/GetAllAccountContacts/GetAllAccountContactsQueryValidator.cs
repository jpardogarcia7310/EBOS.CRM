using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;

public class GetAllAccountContactsQueryValidator : AbstractValidator<GetAllAccountContactsQuery>
{
    public GetAllAccountContactsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
