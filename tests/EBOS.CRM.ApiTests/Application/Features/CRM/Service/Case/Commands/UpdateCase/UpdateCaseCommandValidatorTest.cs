using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;
using FluentValidation.TestHelper;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.UpdateCase;

public class UpdateCaseCommandValidatorTest
{
    private readonly UpdateCaseCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WhenRequestIsValid_Succeeds()
    {
        var command = new UpdateCaseCommand(1, BuildValidRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenStatusIsInvalid_Fails()
    {
        var request = BuildValidRequest() with { Status = "BadStatus" };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.Status);
    }

    [Fact]
    public async Task Validate_WhenPriorityIsInvalid_Fails()
    {
        var request = BuildValidRequest() with { Priority = "BadPriority" };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.Priority);
    }

    [Fact]
    public async Task Validate_WhenTitleIsEmpty_Fails()
    {
        var request = BuildValidRequest() with { Title = "" };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.Title);
    }

    [Fact]
    public async Task Validate_WhenQueueIdIsInvalid_Fails()
    {
        var request = BuildValidRequest() with { QueueId = 0 };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.QueueId);
    }

    [Fact]
    public async Task Validate_WhenSlaIdIsInvalid_Fails()
    {
        var request = BuildValidRequest() with { SlaId = 0 };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.SlaId);
    }

    [Fact]
    public async Task Validate_WhenOwnerUserIdIsInvalid_Fails()
    {
        var request = BuildValidRequest() with { OwnerUserId = 0 };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.OwnerUserId);
    }

    [Fact]
    public async Task Validate_WhenDescriptionIsTooLong_Fails()
    {
        var request = BuildValidRequest() with { Description = new string('a', 2001) };
        var command = new UpdateCaseCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest.Description);
    }

    [Fact]
    public async Task Validate_WhenRequestIsNull_Fails()
    {
        var command = new UpdateCaseCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }

    [Fact]
    public async Task Validate_WhenIdIsInvalid_Fails()
    {
        var command = new UpdateCaseCommand(0, BuildValidRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateCaseRequest BuildValidRequest() => new(
        Id: 1,
        TenantId: 1,
        Title: "Case",
        Description: "Description",
        Status: CaseEntity.StatusOpen,
        Priority: CaseEntity.PriorityLow,
        OwnerUserId: 1,
        QueueId: 1,
        SlaId: 1,
        DueAt: DateTime.UtcNow.AddDays(1)
    );
}


