using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public class GetTenantUsageMetricByIdQueryValidatorTest
{
    private readonly GetTenantUsageMetricByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_PositiveId_Passes()
    {
        var query = new GetTenantUsageMetricByIdQuery(1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantUsageMetricByIdQuery(0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }
}
