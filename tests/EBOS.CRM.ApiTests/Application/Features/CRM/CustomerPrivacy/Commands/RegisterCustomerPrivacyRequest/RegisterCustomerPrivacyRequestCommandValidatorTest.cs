using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;

public class RegisterCustomerPrivacyRequestCommandValidatorTest
{
    private readonly RegisterCustomerPrivacyRequestCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new RegisterCustomerPrivacyRequestCommand(
            new RegisterCustomerPrivacyRequestRequest(1, 10, "ANONYMIZE", "gdpr", true));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidType_Fails()
    {
        var command = new RegisterCustomerPrivacyRequestCommand(
            new RegisterCustomerPrivacyRequestRequest(1, 10, "INVALID", null, false));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Request.RequestType);
    }
}
