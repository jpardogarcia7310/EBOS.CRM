using EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.UpdateLead;

public class UpdateLeadCommandValidatorTest
{
    private readonly UpdateLeadCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var req = new UpdateLeadRequest(1, 1, "WEB", "NEW", 2, "ACME", "John", "john@acme.com", "111", 100m, "notes");
        var result = await _validator.TestValidateAsync(new UpdateLeadCommand(1, req));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new UpdateLeadCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
