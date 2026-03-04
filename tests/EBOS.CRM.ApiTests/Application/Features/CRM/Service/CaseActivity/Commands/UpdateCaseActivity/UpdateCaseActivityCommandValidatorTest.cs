using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;
using EBOS.CRM.Contracts.Requests.CRM.Service.CaseActivity;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;

public class UpdateCaseActivityCommandValidatorTest
{
    private readonly UpdateCaseActivityCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new UpdateCaseActivityCommand(1,
            new UpdateCaseActivityRequest(1, 1, 1, "Activity", "Desc", "Open"));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidStatus_Fails()
    {
        var command = new UpdateCaseActivityCommand(1,
            new UpdateCaseActivityRequest(1, 1, 1, "Activity", "Desc", "INVALID"));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ActivityRequest.Status);
    }
}
