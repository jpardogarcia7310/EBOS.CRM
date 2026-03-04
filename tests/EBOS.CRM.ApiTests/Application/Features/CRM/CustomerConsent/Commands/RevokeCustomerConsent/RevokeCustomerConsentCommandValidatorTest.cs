using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;

public class RevokeCustomerConsentCommandValidatorTest
{
    private readonly RevokeCustomerConsentCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var cmd = new RevokeCustomerConsentCommand(1, new RevokeCustomerConsentRequest(1, DateTime.UtcNow));
        var result = await _validator.TestValidateAsync(cmd);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var cmd = new RevokeCustomerConsentCommand(0, new RevokeCustomerConsentRequest(0, default));
        var result = await _validator.TestValidateAsync(cmd);
        Assert.False(result.IsValid);
    }
}
