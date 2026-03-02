using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public class GetTenantUsageMetricByIdQueryValidatorTest
{
    private readonly GetTenantUsageMetricByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var query = new GetTenantUsageMetricByIdQuery(1);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public async Task Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantUsageMetricByIdQuery(0);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }
}


