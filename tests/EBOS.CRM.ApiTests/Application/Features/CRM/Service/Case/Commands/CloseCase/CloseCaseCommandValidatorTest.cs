using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.CloseCase;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.CloseCase;

public class CloseCaseCommandValidatorTest
{
    private readonly CloseCaseCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenRequestIsValid_Succeeds()
    {
        var command = new CloseCaseCommand(1, new CloseCaseRequest(1, DateTime.UtcNow));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsInvalid_Fails()
    {
        var command = new CloseCaseCommand(0, new CloseCaseRequest(1, DateTime.UtcNow));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenRequestIsNull_Fails()
    {
        var command = new CloseCaseCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }

    [Fact]
    public void Validate_WhenTenantIsInvalid_Fails()
    {
        var command = new CloseCaseCommand(1, new CloseCaseRequest(0, DateTime.UtcNow));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest!.TenantId);
    }

    [Fact]
    public void Validate_WhenClosedAtIsEmpty_Fails()
    {
        var command = new CloseCaseCommand(1, new CloseCaseRequest(1, default));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest!.ClosedAt);
    }

    [Fact]
    public void Validate_WhenIdAndRequestAreInvalid_Fails()
    {
        var command = new CloseCaseCommand(0, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }
}
