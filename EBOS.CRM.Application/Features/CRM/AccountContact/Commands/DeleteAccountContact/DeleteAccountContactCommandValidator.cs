using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public class DeleteAccountContactCommandValidator : AbstractValidator<DeleteAccountContactCommand>
{
    public DeleteAccountContactCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
