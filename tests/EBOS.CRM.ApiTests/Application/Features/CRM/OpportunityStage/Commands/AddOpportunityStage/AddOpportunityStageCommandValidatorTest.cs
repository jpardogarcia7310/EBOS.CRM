using EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;
using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;

public class AddOpportunityStageCommandValidatorTest
{
    private readonly AddOpportunityStageCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddOpportunityStageRequest(1, "Qualified", 1, 0.3m, false, false);
        var result = await _validator.TestValidateAsync(new AddOpportunityStageCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddOpportunityStageCommand(null!));
        Assert.False(result.IsValid);
    }
}
