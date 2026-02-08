using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;

public class GetAllTenantUsageMetricsQueryValidator : AbstractValidator<GetAllTenantUsageMetricsQuery>
{
    public GetAllTenantUsageMetricsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
