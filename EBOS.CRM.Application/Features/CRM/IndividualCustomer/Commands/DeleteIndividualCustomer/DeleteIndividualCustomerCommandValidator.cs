using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.DeleteIndividualCustomer;

public class DeleteIndividualCustomerCommandValidator : AbstractValidator<DeleteIndividualCustomerCommand>
{
    public DeleteIndividualCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
