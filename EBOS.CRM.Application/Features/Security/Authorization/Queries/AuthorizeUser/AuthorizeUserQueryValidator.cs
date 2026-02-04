using FluentValidation;

namespace EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public sealed class AuthorizeUserQueryValidator : AbstractValidator<AuthorizeUserQuery>
{
    public AuthorizeUserQueryValidator()
    {
        RuleFor(x => x.Request.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Request.PolicyCode)
            .NotEmpty()
            .MaximumLength(100);
    }
}
