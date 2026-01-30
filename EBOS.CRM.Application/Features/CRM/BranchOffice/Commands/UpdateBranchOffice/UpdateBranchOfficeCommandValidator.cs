using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;

public class UpdateBranchOfficeCommandValidator : AbstractValidator<UpdateBranchOfficeCommand>
{
    public UpdateBranchOfficeCommandValidator()
    {
        RuleFor(x => x.BranchOfficeRequest).NotNull();
        RuleFor(x => x.BranchOfficeRequest.Id).GreaterThan(0);

        RuleFor(x => x.BranchOfficeRequest.Name)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.BranchOfficeRequest.PhoneNumber)
            .NotEmpty().MaximumLength(20);

        RuleFor(x => x.BranchOfficeRequest.CorporateCustomerId).GreaterThan(0);
    }
}
