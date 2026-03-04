using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;

public class GetAllTenantUsageMetricsQueryValidatorTest
{
    private readonly GetAllTenantUsageMetricsQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllTenantUsageMetricsQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllTenantUsageMetricsQuery(0, 10));
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }
}
