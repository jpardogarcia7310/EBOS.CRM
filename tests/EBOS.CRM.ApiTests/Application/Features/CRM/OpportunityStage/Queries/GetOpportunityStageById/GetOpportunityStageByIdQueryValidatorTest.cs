using EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;

public class GetOpportunityStageByIdQueryValidatorTest
{
    private readonly GetOpportunityStageByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetOpportunityStageByIdQuery(1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetOpportunityStageByIdQuery(0));
        Assert.False(result.IsValid);
    }
}
