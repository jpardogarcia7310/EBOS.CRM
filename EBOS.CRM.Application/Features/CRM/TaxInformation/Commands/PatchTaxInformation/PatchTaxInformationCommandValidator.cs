using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;

public class PatchTaxInformationCommandValidator : AbstractValidator<PatchTaxInformationCommand>
{
    public PatchTaxInformationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TaxInformationRequest).NotNull();

        RuleFor(x => x.TaxInformationRequest)
            .Must(r =>
                r.TaxName != null ||
                r.TaxIdentificationNumber != null ||
                r.CustomerId.HasValue)
            .WithMessage("At least one field must be provided.");

        When(x => x.TaxInformationRequest.TaxName != null, () =>
        {
            RuleFor(x => x.TaxInformationRequest.TaxName!)
                .NotEmpty().MaximumLength(200);
        });

        When(x => x.TaxInformationRequest.TaxIdentificationNumber != null, () =>
        {
            RuleFor(x => x.TaxInformationRequest.TaxIdentificationNumber!)
                .NotEmpty().MaximumLength(20)
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("TaxIdentificationNumber must be alphanumeric.");
        });

        When(x => x.TaxInformationRequest.CustomerId.HasValue, () =>
        {
            RuleFor(x => x.TaxInformationRequest.CustomerId!.Value).GreaterThan(0);
        });
    }
}
