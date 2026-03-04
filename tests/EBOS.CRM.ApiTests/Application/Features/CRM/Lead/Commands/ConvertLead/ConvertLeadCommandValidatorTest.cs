using EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.ConvertLead;

public class ConvertLeadCommandValidatorTest
{
    private readonly ConvertLeadCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new ConvertLeadCommand(1, new ConvertLeadRequest(1, 2, 3, "Opp", 100m, 0.5m, null, null)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new ConvertLeadCommand(0, new ConvertLeadRequest(1, 2, 3, "Opp", 100m, 0.5m, null, null)));
        Assert.False(result.IsValid);
    }
}
