using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;

public class GetTenantConfigurationByIdQueryValidatorTest
{
    private readonly GetTenantConfigurationByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var query = new GetTenantConfigurationByIdQuery(1);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public async Task Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantConfigurationByIdQuery(0);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public async Task Validate_NegativeId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantConfigurationByIdQuery(-5);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }
}


