using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;

public class ExecuteCustomerPrivacyRequestCommandValidatorTest
{
    private readonly ExecuteCustomerPrivacyRequestCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new ExecuteCustomerPrivacyRequestCommand(5, new ExecuteCustomerPrivacyRequestRequest(1));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidTenant_Fails()
    {
        var command = new ExecuteCustomerPrivacyRequestCommand(5, new ExecuteCustomerPrivacyRequestRequest(0));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Request.TenantId);
    }
}
