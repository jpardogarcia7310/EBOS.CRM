using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;

public class GetAccountContactByIdQueryValidator : AbstractValidator<GetAccountContactByIdQuery>
{
    public GetAccountContactByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
