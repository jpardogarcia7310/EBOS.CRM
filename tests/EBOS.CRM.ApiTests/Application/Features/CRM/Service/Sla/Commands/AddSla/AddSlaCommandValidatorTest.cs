using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.AddSla;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Commands.AddSla;

public class AddSlaCommandValidatorTest
{
    private readonly AddSlaCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new AddSlaCommand(new AddSlaRequest(1, "Standard", 60, 30, null, null, true)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddSlaCommand(null!));
        Assert.False(result.IsValid);
    }
}
