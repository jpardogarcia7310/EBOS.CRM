using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;

public sealed class GetCustomerPrivacyRequestByIdQueryValidator : AbstractValidator<GetCustomerPrivacyRequestByIdQuery>
{
    public GetCustomerPrivacyRequestByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TenantId).GreaterThan(0);
    }
}
