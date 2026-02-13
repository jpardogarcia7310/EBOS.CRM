using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryValidator : AbstractValidator<FindCustomerDuplicatesQuery>
{
    public FindCustomerDuplicatesQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.TenantId).GreaterThan(0);
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Request.Email)
                       || !string.IsNullOrWhiteSpace(x.Request.Phone)
                       || !string.IsNullOrWhiteSpace(x.Request.TaxId)
                       || !string.IsNullOrWhiteSpace(x.Request.IdentificationNumber))
            .WithMessage("At least one matching field is required.");
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
