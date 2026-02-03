

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.Address.Commands.DeleteAddress;

public class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




