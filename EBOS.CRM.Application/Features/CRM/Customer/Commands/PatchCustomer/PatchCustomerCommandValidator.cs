using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandValidator : AbstractValidator<PatchCustomerCommand>
{
    public PatchCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CustomerRequest).NotNull();

        When(x => x.CustomerRequest != null, () =>
        {
            RuleFor(x => x.CustomerRequest)
                .Must(r =>
                    r.Code != null ||
                    r.Email != null ||
                    r.Phone != null ||
                    r.StatusId.HasValue)
                .WithMessage("At least one field must be provided.");

            When(x => x.CustomerRequest.Code != null, () =>
            {
                RuleFor(x => x.CustomerRequest.Code!)
                    .NotEmpty().MaximumLength(50);
            });

            When(x => x.CustomerRequest.Email != null, () =>
            {
                RuleFor(x => x.CustomerRequest.Email!)
                    .NotEmpty().MaximumLength(100).EmailAddress();
            });

            When(x => x.CustomerRequest.Phone != null, () =>
            {
                RuleFor(x => x.CustomerRequest.Phone!)
                    .NotEmpty().MaximumLength(12)
                    .Matches(@"^\d+$").WithMessage("Phone must contain only digits.");
            });

            When(x => x.CustomerRequest.StatusId.HasValue, () =>
            {
                RuleFor(x => x.CustomerRequest.StatusId!.Value).GreaterThan(0);
            });
        });
    }
}




