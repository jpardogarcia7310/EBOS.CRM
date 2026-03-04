using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public class GetTenantUsageMetricByIdQueryValidator : AbstractValidator<GetTenantUsageMetricByIdQuery>
{
    public GetTenantUsageMetricByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithMessage("The identifier must be a positive integer greater than 0.");
    }
}
