using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;

public class RetryCustomerPrivacyRequestCommandValidatorTest
{
    private readonly RetryCustomerPrivacyRequestCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new RetryCustomerPrivacyRequestCommand(10, new RetryCustomerPrivacyRequestRequest(1, "retry"));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidTenant_Fails()
    {
        var command = new RetryCustomerPrivacyRequestCommand(10, new RetryCustomerPrivacyRequestRequest(0, null));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Request.TenantId);
    }
}
