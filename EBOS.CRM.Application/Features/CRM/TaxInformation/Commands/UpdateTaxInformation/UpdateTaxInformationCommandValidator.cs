using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public class UpdateTaxInformationCommandValidator : AbstractValidator<UpdateTaxInformationCommand>
{
    public UpdateTaxInformationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TaxInformationRequest).NotNull();
        RuleFor(x => x.TaxInformationRequest.TaxName).NotEmpty(); 
        RuleFor(x => x.TaxInformationRequest.TaxIdentificationNumber).NotEmpty();
        RuleFor(x => x.TaxInformationRequest.CustomerId).GreaterThan(0);
    }
}




