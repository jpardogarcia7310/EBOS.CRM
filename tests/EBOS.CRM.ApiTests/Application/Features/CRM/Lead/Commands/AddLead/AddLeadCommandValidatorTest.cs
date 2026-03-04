using EBOS.CRM.Application.Features.CRM.Lead.Commands.AddLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.AddLead;

public class AddLeadCommandValidatorTest
{
    private readonly AddLeadCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddLeadRequest(1, "WEB", "NEW", 10, "ACME", "John Doe", "john@acme.com", "111111111", 1000m, "notes");
        var result = await _validator.TestValidateAsync(new AddLeadCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddLeadCommand(null!));
        Assert.False(result.IsValid);
    }
}
