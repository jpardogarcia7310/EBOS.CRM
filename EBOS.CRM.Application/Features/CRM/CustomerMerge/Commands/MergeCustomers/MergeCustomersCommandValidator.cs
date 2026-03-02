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
        RuleFor(x => x.Request)
            .Must(r => r.MergeCustomerIds.All(id => id > 0))
            .WithMessage("MergeCustomerIds must contain only positive values.");
        RuleFor(x => x.Request)
            .Must(r => !r.MergeCustomerIds.Contains(r.WinnerCustomerId))
            .WithMessage("MergeCustomerIds must not include WinnerCustomerId.");
        RuleFor(x => x.Request.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}
