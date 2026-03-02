using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandValidatorTest
{
    private readonly AddCustomerConsentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidGrantEvent_Passes()
    {
        var command = new AddCustomerConsentCommand(new AddCustomerConsentRequest(
            1, 100, "MARKETING_EMAIL", true, DateTime.UtcNow, "web", null));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ExpireEventWithoutExpiresAt_Fails()
    {
        var now = DateTime.UtcNow;
        var command = new AddCustomerConsentCommand(new AddCustomerConsentRequest(
            1, 100, "MARKETING_EMAIL", false, now, "policy", null));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConsentRequest.ExpiresAt);
    }

    [Fact]
    public void Validate_ExpireEventWithDifferentTimestamp_Fails()
    {
        var now = DateTime.UtcNow;
        var command = new AddCustomerConsentCommand(new AddCustomerConsentRequest(
            1, 100, "MARKETING_EMAIL", false, now, "policy", now.AddMinutes(1)));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConsentRequest.ExpiresAt);
    }
}
