using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.DeleteTaxInformationAddress;

public class DeleteTaxInformationAddressCommandValidator : AbstractValidator<DeleteTaxInformationAddressCommand>
{
    public DeleteTaxInformationAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
