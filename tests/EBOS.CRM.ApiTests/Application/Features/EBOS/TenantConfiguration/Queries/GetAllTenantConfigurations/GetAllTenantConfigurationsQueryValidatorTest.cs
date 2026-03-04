using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;

public class GetAllTenantConfigurationsQueryValidatorTest
{
    private readonly GetAllTenantConfigurationsQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllTenantConfigurationsQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllTenantConfigurationsQuery(0, 10));
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }
}
