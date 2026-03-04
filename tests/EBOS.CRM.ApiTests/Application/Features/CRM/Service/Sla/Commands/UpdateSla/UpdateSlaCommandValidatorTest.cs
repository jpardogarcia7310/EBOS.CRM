using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.UpdateSla;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Commands.UpdateSla;

public class UpdateSlaCommandValidatorTest
{
    private readonly UpdateSlaCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new UpdateSlaCommand(1, new UpdateSlaRequest(1, 1, "Updated", 60, 30, null, null, true)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new UpdateSlaCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
