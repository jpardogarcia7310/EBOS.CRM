using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.ReopenCase;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.ReopenCase;

public class ReopenCaseCommandValidatorTest
{
    private readonly ReopenCaseCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenRequestIsValid_Succeeds()
    {
        var command = new ReopenCaseCommand(1, new ReopenCaseRequest(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsInvalid_Fails()
    {
        var command = new ReopenCaseCommand(0, new ReopenCaseRequest(1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenRequestIsNull_Fails()
    {
        var command = new ReopenCaseCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }

    [Fact]
    public void Validate_WhenTenantIsInvalid_Fails()
    {
        var command = new ReopenCaseCommand(1, new ReopenCaseRequest(0));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest!.TenantId);
    }

    [Fact]
    public void Validate_WhenIdAndRequestAreInvalid_Fails()
    {
        var command = new ReopenCaseCommand(0, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }
}
