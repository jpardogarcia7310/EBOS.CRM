using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.DeleteTaxInformation;

public class DeleteTaxInformationCommandValidator : AbstractValidator<DeleteTaxInformationCommand>
{
    public DeleteTaxInformationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




