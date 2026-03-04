using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.DeleteBankInformation;

public class DeleteBankInformationCommandValidator : AbstractValidator<DeleteBankInformationCommand>
{
    public DeleteBankInformationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




