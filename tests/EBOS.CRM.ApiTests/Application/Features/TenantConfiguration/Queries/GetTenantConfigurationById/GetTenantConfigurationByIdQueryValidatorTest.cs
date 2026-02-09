using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.TenantConfiguration.Queries.GetTenantConfigurationById;

public class GetTenantConfigurationByIdQueryValidatorTest
{
    private readonly GetTenantConfigurationByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_PositiveId_Passes()
    {
        var query = new GetTenantConfigurationByIdQuery(1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantConfigurationByIdQuery(0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public void Validate_NegativeId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantConfigurationByIdQuery(-5);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }
}
