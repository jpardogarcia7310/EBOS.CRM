using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.UpdateCustomerAddress;

public class UpdateCustomerAddressCommandValidator : AbstractValidator<UpdateCustomerAddressCommand>
{
    public UpdateCustomerAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CustomerAddressRequest).NotNull();


        RuleFor(x => x.CustomerAddressRequest.CustomerId).GreaterThan(0); RuleFor(x => x.CustomerAddressRequest.AddressId).GreaterThan(0);
    }
}
