using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;

public class AddTaxInformationCommandValidator : AbstractValidator<AddTaxInformationCommand>
{
    public AddTaxInformationCommandValidator()
    {
        RuleFor(x => x.TaxInformationRequest).NotNull();

        RuleFor(x => x.TaxInformationRequest.TaxName)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.TaxInformationRequest.TaxIdentificationNumber)
            .NotEmpty().MaximumLength(20)
            .Matches(@"^[A-Za-z0-9]+$").WithMessage("TaxIdentificationNumber must be alphanumeric.");

        RuleFor(x => x.TaxInformationRequest.CustomerId).GreaterThan(0);
    }
}
