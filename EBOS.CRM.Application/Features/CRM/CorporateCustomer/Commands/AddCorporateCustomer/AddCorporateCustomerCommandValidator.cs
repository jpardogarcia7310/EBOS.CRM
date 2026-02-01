using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;

public class AddCorporateCustomerCommandValidator : AbstractValidator<AddCorporateCustomerCommand>
{
    public AddCorporateCustomerCommandValidator()
    {
        RuleFor(x => x.CorporateCustomerRequest).NotNull();
        RuleFor(x => x.CorporateCustomerRequest.Code).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.Email).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.Phone).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.LegalName).NotEmpty(); RuleFor(x => x.CorporateCustomerRequest.TaxIdentification).NotEmpty();

        RuleFor(x => x.CorporateCustomerRequest.StatusId).GreaterThan(0);
    }
}
