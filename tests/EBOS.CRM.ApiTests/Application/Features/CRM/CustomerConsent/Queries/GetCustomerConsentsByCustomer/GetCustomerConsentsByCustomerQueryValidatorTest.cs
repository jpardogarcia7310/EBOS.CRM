using EBOS.CRM.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;

public class GetCustomerConsentsByCustomerQueryValidatorTest
{
    private readonly GetCustomerConsentsByCustomerQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetCustomerConsentsByCustomerQuery(1, 2, 1, 20));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetCustomerConsentsByCustomerQuery(0, 0, 0, 0));
        Assert.False(result.IsValid);
    }
}
