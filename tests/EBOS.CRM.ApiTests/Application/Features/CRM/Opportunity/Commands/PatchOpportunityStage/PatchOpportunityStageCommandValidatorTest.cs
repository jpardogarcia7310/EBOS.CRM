using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;

public class PatchOpportunityStageCommandValidatorTest
{
    private readonly PatchOpportunityStageCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new PatchOpportunityStageCommand(1, new PatchOpportunityStageRequest(1, 2, 0.5m)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new PatchOpportunityStageCommand(0, new PatchOpportunityStageRequest(1, 2, 0.5m)));
        Assert.False(result.IsValid);
    }
}
