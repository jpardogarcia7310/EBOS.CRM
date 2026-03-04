using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.AddOpportunity;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.AddOpportunity;

public class AddOpportunityCommandValidatorTest
{
    private readonly AddOpportunityCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddOpportunityRequest(1, "Opp A", 1, 2, 3, DateTime.UtcNow.AddDays(30), 1000m, 0.5m, "WEB", null);
        var result = await _validator.TestValidateAsync(new AddOpportunityCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddOpportunityCommand(null!));
        Assert.False(result.IsValid);
    }
}
