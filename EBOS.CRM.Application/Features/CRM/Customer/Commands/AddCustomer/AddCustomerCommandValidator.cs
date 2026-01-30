using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandValidator : AbstractValidator<AddCustomerCommand>
{
    public AddCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerRequest).NotNull();

        RuleFor(x => x.CustomerRequest.Code)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.CustomerRequest.Email)
            .NotEmpty().MaximumLength(100).EmailAddress();

        RuleFor(x => x.CustomerRequest.Phone)
            .NotEmpty().MaximumLength(12)
            .Matches(@"^\d+$").WithMessage("Phone must contain only digits.");

        RuleFor(x => x.CustomerRequest.StatusId).GreaterThan(0);
    }
}
