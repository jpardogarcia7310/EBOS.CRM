using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public class DeleteAccountContactRoleCommandValidator : AbstractValidator<DeleteAccountContactRoleCommand>
{
    public DeleteAccountContactRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
