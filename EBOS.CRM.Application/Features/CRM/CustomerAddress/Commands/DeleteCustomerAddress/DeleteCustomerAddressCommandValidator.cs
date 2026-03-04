using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.DeleteCustomerAddress;

public class DeleteCustomerAddressCommandValidator : AbstractValidator<DeleteCustomerAddressCommand>
{
    public DeleteCustomerAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




