using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;

public class GetCustomerPrivacyRequestByIdQueryValidatorTest
{
    private readonly GetCustomerPrivacyRequestByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var query = new GetCustomerPrivacyRequestByIdQuery(1, 1);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
