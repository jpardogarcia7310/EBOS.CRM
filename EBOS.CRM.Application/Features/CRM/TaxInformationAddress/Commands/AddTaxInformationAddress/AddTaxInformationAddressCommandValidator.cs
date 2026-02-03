

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.AddTaxInformationAddress;

public class AddTaxInformationAddressCommandValidator : AbstractValidator<AddTaxInformationAddressCommand>
{
    public AddTaxInformationAddressCommandValidator()
    {
        RuleFor(x => x.TaxInformationAddressRequest).NotNull();


        RuleFor(x => x.TaxInformationAddressRequest.TaxInformationId).GreaterThan(0); RuleFor(x => x.TaxInformationAddressRequest.AddressId).GreaterThan(0);
    }
}




