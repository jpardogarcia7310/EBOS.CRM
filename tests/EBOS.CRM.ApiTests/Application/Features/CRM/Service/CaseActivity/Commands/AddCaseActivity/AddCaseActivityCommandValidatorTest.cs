using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;
using EBOS.CRM.Contracts.Requests.CRM.Service.CaseActivity;
using FluentValidation.TestHelper;
using CaseActivityEntity = EBOS.CRM.Domain.Entities.CRM.CaseActivity;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;

public class AddCaseActivityCommandValidatorTest
{
    private readonly AddCaseActivityCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddCaseActivityRequest(1, 10, "Activity", "Desc", CaseActivityEntity.StatusOpen);
        var result = await _validator.TestValidateAsync(new AddCaseActivityCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddCaseActivityCommand(null!));
        Assert.False(result.IsValid);
    }
}
