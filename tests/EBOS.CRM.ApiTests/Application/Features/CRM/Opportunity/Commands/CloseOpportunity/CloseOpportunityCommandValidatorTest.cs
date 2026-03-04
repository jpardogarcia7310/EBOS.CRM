using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;

public class CloseOpportunityCommandValidatorTest
{
    private readonly CloseOpportunityCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new CloseOpportunityCommand(1, new CloseOpportunityRequest(1, 2, true, "won")));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new CloseOpportunityCommand(0, new CloseOpportunityRequest(1, 2, true, "won")));
        Assert.False(result.IsValid);
    }
}
