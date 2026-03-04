using EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.DisqualifyLead;

public class DisqualifyLeadCommandValidatorTest
{
    private readonly DisqualifyLeadCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new DisqualifyLeadCommand(1, new DisqualifyLeadRequest(1, "reason")));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new DisqualifyLeadCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
