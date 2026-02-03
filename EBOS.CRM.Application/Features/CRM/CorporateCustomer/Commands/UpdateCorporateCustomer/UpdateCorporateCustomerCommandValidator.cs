using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;

public class UpdateCorporateCustomerCommandValidator : AbstractValidator<UpdateCorporateCustomerCommand>
{
    public UpdateCorporateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CorporateCustomerRequest).NotNull();
        RuleFor(x => x.CorporateCustomerRequest.Code).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.Email).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.Phone).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.LegalName).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.TaxIdentification).NotEmpty();

        RuleFor(x => x.CorporateCustomerRequest.StatusId).GreaterThan(0);
    }
}




