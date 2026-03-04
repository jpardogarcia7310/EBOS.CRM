using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;

public class UpdateOpportunityCommandValidatorTest
{
    private readonly UpdateOpportunityCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var req = new UpdateOpportunityRequest(1, 1, "Opp", 2, 3, 4, null, 100m, 0.5m, null, null, null);
        var result = await _validator.TestValidateAsync(new UpdateOpportunityCommand(1, req));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new UpdateOpportunityCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
