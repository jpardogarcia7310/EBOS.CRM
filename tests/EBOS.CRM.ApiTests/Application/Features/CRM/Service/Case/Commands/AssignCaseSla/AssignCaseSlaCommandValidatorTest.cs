using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;

public class AssignCaseSlaCommandValidatorTest
{
    private readonly AssignCaseSlaCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AssignCaseSlaCommand(1, new AssignCaseSlaRequest(1, 7));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidValues_Fails()
    {
        var command = new AssignCaseSlaCommand(0, new AssignCaseSlaRequest(0, 0));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.TenantId);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.SlaId);
    }
}
