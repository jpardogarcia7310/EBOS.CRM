using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public class DeleteAccountContactRoleCommandValidator : AbstractValidator<DeleteAccountContactRoleCommand>
{
    public DeleteAccountContactRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AccountContactRoleRequest).NotNull();
        RuleFor(x => x.AccountContactRoleRequest.TenantId).GreaterThan(0);
    }
}
