using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.ToggleSla;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Commands.ToggleSla;

public class ToggleSlaCommandValidatorTest
{
    private readonly ToggleSlaCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new ToggleSlaCommand(1, new ToggleSlaRequest(1, false)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new ToggleSlaCommand(0, new ToggleSlaRequest(1, false)));
        Assert.False(result.IsValid);
    }
}
