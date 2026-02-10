using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CustomerRequest).NotNull();
        When(x => x.CustomerRequest != null, () =>
        {
            RuleFor(x => x.CustomerRequest.Code).NotEmpty(); 
            RuleFor(x => x.CustomerRequest.Email).NotEmpty(); 
            RuleFor(x => x.CustomerRequest.Phone).NotEmpty();
            RuleFor(x => x.CustomerRequest.StatusId).GreaterThan(0);
        });
    }
}




