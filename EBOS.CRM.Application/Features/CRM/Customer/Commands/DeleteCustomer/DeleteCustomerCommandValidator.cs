using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.DeleteCustomer;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
