using EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.QualifyLead;

public class QualifyLeadCommandValidatorTest
{
    private readonly QualifyLeadCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new QualifyLeadCommand(1, new QualifyLeadRequest(1, "notes")));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new QualifyLeadCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
