using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;

public class AddTaxInformationCommandValidator : AbstractValidator<AddTaxInformationCommand>
{
    public AddTaxInformationCommandValidator()
    {
        RuleFor(x => x.TaxInformationRequest).NotNull();
        RuleFor(x => x.TaxInformationRequest.TaxName).NotEmpty(); RuleFor(x => x.TaxInformationRequest.TaxIdentificationNumber).NotEmpty();

        RuleFor(x => x.TaxInformationRequest.CustomerId).GreaterThan(0);
    }
}
