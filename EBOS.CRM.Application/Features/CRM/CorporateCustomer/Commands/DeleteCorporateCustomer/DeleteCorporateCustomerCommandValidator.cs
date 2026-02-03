using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.DeleteCorporateCustomer;

public class DeleteCorporateCustomerCommandValidator : AbstractValidator<DeleteCorporateCustomerCommand>
{
    public DeleteCorporateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




