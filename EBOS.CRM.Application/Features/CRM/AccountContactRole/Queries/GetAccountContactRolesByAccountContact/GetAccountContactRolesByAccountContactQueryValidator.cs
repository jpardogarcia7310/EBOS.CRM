using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;

public class GetAccountContactRolesByAccountContactQueryValidator : AbstractValidator<GetAccountContactRolesByAccountContactQuery>
{
    public GetAccountContactRolesByAccountContactQueryValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.AccountContactId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
