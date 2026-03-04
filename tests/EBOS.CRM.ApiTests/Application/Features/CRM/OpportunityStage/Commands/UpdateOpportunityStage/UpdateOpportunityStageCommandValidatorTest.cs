using EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;
using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;

public class UpdateOpportunityStageCommandValidatorTest
{
    private readonly UpdateOpportunityStageCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var req = new UpdateOpportunityStageRequest(1, 1, "Qualified", 1, 0.3m, false, false);
        var result = await _validator.TestValidateAsync(new UpdateOpportunityStageCommand(1, req));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new UpdateOpportunityStageCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
