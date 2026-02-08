using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandValidator : AbstractValidator<AddCustomerCommand>
{
    public AddCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerRequest).NotNull();
        When(x => x.CustomerRequest != null, () =>
        {
            RuleFor(x => x.CustomerRequest.Code).NotEmpty(); RuleFor(x => x.CustomerRequest.Email).NotEmpty(); RuleFor(x => x.CustomerRequest.Phone).NotEmpty();

            RuleFor(x => x.CustomerRequest.StatusId).GreaterThan(0);
        });
    }
}




