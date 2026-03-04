using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.UpdateBranchOfficeAddress;

public class UpdateBranchOfficeAddressCommandValidator : AbstractValidator<UpdateBranchOfficeAddressCommand>
{
    public UpdateBranchOfficeAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.BranchOfficeAddressRequest).NotNull();
        RuleFor(x => x.BranchOfficeAddressRequest.BranchOfficeId).GreaterThan(0);
        RuleFor(x => x.BranchOfficeAddressRequest.AddressId).GreaterThan(0);
    }
}




