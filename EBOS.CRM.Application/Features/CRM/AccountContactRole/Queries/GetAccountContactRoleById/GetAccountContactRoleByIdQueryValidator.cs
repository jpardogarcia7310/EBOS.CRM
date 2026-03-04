using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;

public class GetAccountContactRoleByIdQueryValidator : AbstractValidator<GetAccountContactRoleByIdQuery>
{
    public GetAccountContactRoleByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
