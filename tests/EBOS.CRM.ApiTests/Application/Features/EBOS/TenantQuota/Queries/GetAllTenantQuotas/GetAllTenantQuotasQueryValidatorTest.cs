using EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;

public class GetAllTenantQuotasQueryValidatorTest
{
    private readonly GetAllTenantQuotasQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllTenantQuotasQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllTenantQuotasQuery(0, 10));
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }
}
