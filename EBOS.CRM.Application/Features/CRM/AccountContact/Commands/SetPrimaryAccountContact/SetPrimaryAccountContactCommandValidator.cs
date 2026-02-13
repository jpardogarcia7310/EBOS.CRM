using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;

public class SetPrimaryAccountContactCommandValidator : AbstractValidator<SetPrimaryAccountContactCommand>
{
    public SetPrimaryAccountContactCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AccountContactRequest).NotNull();
        RuleFor(x => x.AccountContactRequest.TenantId).GreaterThan(0);
    }
}
