using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;

public class GetAllTenantQuotasQueryValidator : AbstractValidator<GetAllTenantQuotasQuery>
{
    public GetAllTenantQuotasQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
