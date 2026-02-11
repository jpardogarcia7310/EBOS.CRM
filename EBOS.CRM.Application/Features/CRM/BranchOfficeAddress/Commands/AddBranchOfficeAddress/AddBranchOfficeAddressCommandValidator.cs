using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.AddBranchOfficeAddress;

public class AddBranchOfficeAddressCommandValidator : AbstractValidator<AddBranchOfficeAddressCommand>
{
    public AddBranchOfficeAddressCommandValidator()
    {
        RuleFor(x => x.BranchOfficeAddressRequest).NotNull();
        RuleFor(x => x.BranchOfficeAddressRequest.BranchOfficeId).GreaterThan(0);
        RuleFor(x => x.BranchOfficeAddressRequest.AddressId).GreaterThan(0);
    }
}




