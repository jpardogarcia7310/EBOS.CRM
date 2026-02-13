using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;

public class MergeCustomersCommandValidator : AbstractValidator<MergeCustomersCommand>
{
    public MergeCustomersCommandValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.TenantId).GreaterThan(0);
        RuleFor(x => x.Request.WinnerCustomerId).GreaterThan(0);
        RuleFor(x => x.Request.MergeCustomerIds)
            .NotNull()
            .Must(ids => ids.Count > 0)
            .WithMessage("MergeCustomerIds must not be empty.");
    }
}
