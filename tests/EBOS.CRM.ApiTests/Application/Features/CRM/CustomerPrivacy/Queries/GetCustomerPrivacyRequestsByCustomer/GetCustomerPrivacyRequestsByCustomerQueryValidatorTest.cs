using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;

public class GetCustomerPrivacyRequestsByCustomerQueryValidatorTest
{
    private readonly GetCustomerPrivacyRequestsByCustomerQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new GetCustomerPrivacyRequestsByCustomerQuery(1, 2, 1, 10));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidTenant_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new GetCustomerPrivacyRequestsByCustomerQuery(0, 2, 1, 10));
        Assert.False(result.IsValid);
    }
}
