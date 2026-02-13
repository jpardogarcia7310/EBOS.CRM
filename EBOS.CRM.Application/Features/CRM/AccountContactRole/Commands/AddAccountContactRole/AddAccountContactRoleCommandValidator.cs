using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;

public class AddAccountContactRoleCommandValidator : AbstractValidator<AddAccountContactRoleCommand>
{
    public AddAccountContactRoleCommandValidator()
    {
        RuleFor(x => x.AccountContactRoleRequest).NotNull();
        RuleFor(x => x.AccountContactRoleRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.AccountContactRoleRequest.AccountContactId).GreaterThan(0);
        RuleFor(x => x.AccountContactRoleRequest.RoleCode).NotEmpty();
        RuleFor(x => x.AccountContactRoleRequest.ValidFrom).NotEmpty();
        RuleFor(x => x.AccountContactRoleRequest.ValidTo)
            .Must((request, validTo) => !validTo.HasValue || validTo.Value >= request.AccountContactRoleRequest.ValidFrom)
            .WithMessage("ValidTo cannot be earlier than ValidFrom.");
    }
}
