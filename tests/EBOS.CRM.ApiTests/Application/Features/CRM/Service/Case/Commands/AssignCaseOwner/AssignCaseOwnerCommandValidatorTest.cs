using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;

public class AssignCaseOwnerCommandValidatorTest
{
    private readonly AssignCaseOwnerCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AssignCaseOwnerCommand(1, new AssignCaseOwnerRequest(1, 10));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidValues_Fails()
    {
        var command = new AssignCaseOwnerCommand(0, new AssignCaseOwnerRequest(0, 0));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.TenantId);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.OwnerUserId);
    }
}
