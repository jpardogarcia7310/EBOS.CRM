

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.DeleteBranchOffice;

public class DeleteBranchOfficeCommandValidator : AbstractValidator<DeleteBranchOfficeCommand>
{
    public DeleteBranchOfficeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




