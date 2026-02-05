using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.AddBankInformation;

public class AddBankInformationCommandValidator : AbstractValidator<AddBankInformationCommand>
{
    public AddBankInformationCommandValidator()
    {
        RuleFor(x => x.BankInformationRequest).NotNull();
        RuleFor(x => x.BankInformationRequest.Iban).NotEmpty();
        RuleFor(x => x.BankInformationRequest.Bic).MaximumLength(500); 
        RuleFor(x => x.BankInformationRequest.BankName).MaximumLength(500);
        RuleFor(x => x.BankInformationRequest.CustomerId).GreaterThan(0);
    }
}




