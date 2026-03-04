using FluentValidation;

namespace EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public sealed class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
    {
        RuleFor(x => x.Request.ExternalId)
            .NotEmpty()
            .MaximumLength(128);
        RuleFor(x => x.Request.Username)
            .NotEmpty()
            .MaximumLength(64);
        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        RuleFor(x => x.Request.DisplayName)
            .NotEmpty()
            .MaximumLength(120);
    }
}
