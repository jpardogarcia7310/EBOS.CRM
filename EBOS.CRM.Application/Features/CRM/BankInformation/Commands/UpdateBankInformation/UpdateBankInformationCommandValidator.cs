using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;

public class UpdateBankInformationCommandValidator : AbstractValidator<UpdateBankInformationCommand>
{
    public UpdateBankInformationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.BankInformationRequest).NotNull();

        When(x => x.BankInformationRequest != null, () =>
        {
            RuleFor(x => x.BankInformationRequest.Iban).NotEmpty();
            RuleFor(x => x.BankInformationRequest.Bic).MaximumLength(500); 
            RuleFor(x => x.BankInformationRequest.BankName).MaximumLength(500);
            RuleFor(x => x.BankInformationRequest.CustomerId).GreaterThan(0);
        });
    }
}




