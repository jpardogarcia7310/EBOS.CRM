using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;

public class AddCustomerAddressCommandValidator : AbstractValidator<AddCustomerAddressCommand>
{
    public AddCustomerAddressCommandValidator()
    {
        RuleFor(x => x.CustomerAddressRequest).NotNull();


        RuleFor(x => x.CustomerAddressRequest.CustomerId).GreaterThan(0); RuleFor(x => x.CustomerAddressRequest.AddressId).GreaterThan(0);
    }
}
