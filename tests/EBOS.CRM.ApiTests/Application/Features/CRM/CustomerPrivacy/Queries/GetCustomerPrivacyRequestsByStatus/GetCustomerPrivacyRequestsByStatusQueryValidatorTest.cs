using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByStatus;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByStatus;

public class GetCustomerPrivacyRequestsByStatusQueryValidatorTest
{
    private readonly GetCustomerPrivacyRequestsByStatusQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var query = new GetCustomerPrivacyRequestsByStatusQuery(1, "FAILED", 1, 50);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidStatus_Fails()
    {
        var query = new GetCustomerPrivacyRequestsByStatusQuery(1, "UNKNOWN", 1, 50);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
