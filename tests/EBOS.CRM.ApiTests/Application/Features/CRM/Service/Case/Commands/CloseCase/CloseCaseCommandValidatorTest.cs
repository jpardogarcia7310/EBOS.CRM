using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.CloseCase;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.CloseCase;

public class CloseCaseCommandValidatorTest
{
    private readonly CloseCaseCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WhenRequestIsValid_Succeeds()
    {
        var command = new CloseCaseCommand(1, new CloseCaseRequest(1, DateTime.UtcNow));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenIdIsInvalid_Fails()
    {
        var command = new CloseCaseCommand(0, new CloseCaseRequest(1, DateTime.UtcNow));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_WhenRequestIsNull_Fails()
    {
        var command = new CloseCaseCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }

    [Fact]
    public async Task Validate_WhenTenantIsInvalid_Fails()
    {
        var command = new CloseCaseCommand(1, new CloseCaseRequest(0, DateTime.UtcNow));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest!.TenantId);
    }

    [Fact]
    public async Task Validate_WhenClosedAtIsEmpty_Fails()
    {
        var command = new CloseCaseCommand(1, new CloseCaseRequest(1, default));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest!.ClosedAt);
    }

    [Fact]
    public async Task Validate_WhenIdAndRequestAreInvalid_Fails()
    {
        var command = new CloseCaseCommand(0, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }
}


