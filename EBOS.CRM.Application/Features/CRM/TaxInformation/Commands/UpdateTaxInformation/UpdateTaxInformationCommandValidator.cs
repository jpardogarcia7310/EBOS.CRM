using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public class UpdateTaxInformationCommandValidator : AbstractValidator<UpdateTaxInformationCommand>
{
    public UpdateTaxInformationCommandValidator()
    {
        RuleFor(x => x.TaxInformationRequest).NotNull();
        RuleFor(x => x.TaxInformationRequest.Id).GreaterThan(0);

        RuleFor(x => x.TaxInformationRequest.TaxName)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.TaxInformationRequest.TaxIdentificationNumber)
            .NotEmpty().MaximumLength(20)
            .Matches(@"^[A-Za-z0-9]+$").WithMessage("TaxIdentificationNumber must be alphanumeric.");

        RuleFor(x => x.TaxInformationRequest.CustomerId).GreaterThan(0);
    }
}
