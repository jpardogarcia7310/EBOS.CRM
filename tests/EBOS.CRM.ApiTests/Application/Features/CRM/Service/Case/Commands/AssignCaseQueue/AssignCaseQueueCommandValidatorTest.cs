using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;

public class AssignCaseQueueCommandValidatorTest
{
    private readonly AssignCaseQueueCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AssignCaseQueueCommand(1, new AssignCaseQueueRequest(1, 5));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidValues_Fails()
    {
        var command = new AssignCaseQueueCommand(0, new AssignCaseQueueRequest(0, 0));
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.TenantId);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.QueueId);
    }
}
