using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;

public class UpdateTaxInformationAddressCommandValidator : AbstractValidator<UpdateTaxInformationAddressCommand>
{
    public UpdateTaxInformationAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TaxInformationAddressRequest).NotNull();
        RuleFor(x => x.TaxInformationAddressRequest.TaxInformationId).GreaterThan(0);
        RuleFor(x => x.TaxInformationAddressRequest.AddressId).GreaterThan(0);
    }
}




