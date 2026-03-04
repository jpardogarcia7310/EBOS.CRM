using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.DeleteBranchOfficeAddress;

public class DeleteBranchOfficeAddressCommandValidator : AbstractValidator<DeleteBranchOfficeAddressCommand>
{
    public DeleteBranchOfficeAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




