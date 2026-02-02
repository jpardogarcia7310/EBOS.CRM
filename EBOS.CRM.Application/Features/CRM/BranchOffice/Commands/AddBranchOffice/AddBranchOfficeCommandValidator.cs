using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public class AddBranchOfficeCommandValidator : AbstractValidator<AddBranchOfficeCommand>
{
    public AddBranchOfficeCommandValidator()
    {
        RuleFor(x => x.BranchOfficeRequest).NotNull();
        RuleFor(x => x.BranchOfficeRequest.Name).NotEmpty(); RuleFor(x => x.BranchOfficeRequest.PhoneNumber).NotEmpty();

        RuleFor(x => x.BranchOfficeRequest.CorporateCustomerId).GreaterThan(0);
    }
}
