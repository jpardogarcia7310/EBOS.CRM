using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public class DeleteAccountContactCommandValidator : AbstractValidator<DeleteAccountContactCommand>
{
    public DeleteAccountContactCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AccountContactRequest).NotNull();
        RuleFor(x => x.AccountContactRequest.TenantId).GreaterThan(0);
    }
}
