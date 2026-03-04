using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.ReopenCase;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.ReopenCase;

public class ReopenCaseCommandValidatorTest
{
    private readonly ReopenCaseCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WhenRequestIsValid_Succeeds()
    {
        var command = new ReopenCaseCommand(1, new ReopenCaseRequest(1));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenIdIsInvalid_Fails()
    {
        var command = new ReopenCaseCommand(0, new ReopenCaseRequest(1));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_WhenRequestIsNull_Fails()
    {
        var command = new ReopenCaseCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }

    [Fact]
    public async Task Validate_WhenTenantIsInvalid_Fails()
    {
        var command = new ReopenCaseCommand(1, new ReopenCaseRequest(0));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest!.TenantId);
    }

    [Fact]
    public async Task Validate_WhenIdAndRequestAreInvalid_Fails()
    {
        var command = new ReopenCaseCommand(0, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }
}


