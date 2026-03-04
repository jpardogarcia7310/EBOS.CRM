using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandValidatorTest
{
    private readonly AddCustomerConsentCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidGrantEvent_Passes()
    {
        var command = new AddCustomerConsentCommand(new AddCustomerConsentRequest(
            1, 100, "MARKETING_EMAIL", true, DateTime.UtcNow, "web", null));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ExpireEventWithoutExpiresAt_Fails()
    {
        var now = DateTime.UtcNow;
        var command = new AddCustomerConsentCommand(new AddCustomerConsentRequest(
            1, 100, "MARKETING_EMAIL", false, now, "policy", null));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ConsentRequest.ExpiresAt);
    }

    [Fact]
    public async Task Validate_ExpireEventWithDifferentTimestamp_Fails()
    {
        var now = DateTime.UtcNow;
        var command = new AddCustomerConsentCommand(new AddCustomerConsentRequest(
            1, 100, "MARKETING_EMAIL", false, now, "policy", now.AddMinutes(1)));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ConsentRequest.ExpiresAt);
    }
}


