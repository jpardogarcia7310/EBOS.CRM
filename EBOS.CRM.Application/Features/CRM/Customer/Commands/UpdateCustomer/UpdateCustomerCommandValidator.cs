using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerRequest).NotNull();

        RuleFor(x => x.CustomerRequest.Id).GreaterThan(0);

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
