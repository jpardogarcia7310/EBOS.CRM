using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public class PatchBranchOfficeCommandValidator : AbstractValidator<PatchBranchOfficeCommand>
{
    public PatchBranchOfficeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.BranchOfficeRequest).NotNull();

        RuleFor(x => x.BranchOfficeRequest)
            .Must(r =>
                r.Name != null ||
                r.PhoneNumber != null ||
                r.CorporateCustomerId.HasValue)
            .WithMessage("At least one field must be provided.");

        When(x => x.BranchOfficeRequest.Name != null, () =>
        {
            RuleFor(x => x.BranchOfficeRequest.Name!)
                .NotEmpty().MaximumLength(200);
        });

        When(x => x.BranchOfficeRequest.PhoneNumber != null, () =>
        {
            RuleFor(x => x.BranchOfficeRequest.PhoneNumber!)
                .NotEmpty().MaximumLength(20);
        });

        When(x => x.BranchOfficeRequest.CorporateCustomerId.HasValue, () =>
        {
            RuleFor(x => x.BranchOfficeRequest.CorporateCustomerId!.Value).GreaterThan(0);
        });
    }
}
