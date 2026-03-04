using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AddCase;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using FluentValidation.TestHelper;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.AddCase;

public class AddCaseCommandValidatorTest
{
    private readonly AddCaseCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddCaseRequest(1, "Case", "Desc", CaseEntity.StatusOpen, CaseEntity.PriorityLow, 10, 20, 30, null);
        var result = await _validator.TestValidateAsync(new AddCaseCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddCaseCommand(null!));
        Assert.False(result.IsValid);
    }
}
