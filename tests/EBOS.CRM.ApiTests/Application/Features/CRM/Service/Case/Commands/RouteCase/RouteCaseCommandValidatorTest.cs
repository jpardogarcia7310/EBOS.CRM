using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.RouteCase;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.RouteCase;

public class RouteCaseCommandValidatorTest
{
    private readonly RouteCaseCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WhenRequestIsValid_Succeeds()
    {
        var command = new RouteCaseCommand(1, new RouteCaseRequest(1));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenIdIsInvalid_Fails()
    {
        var command = new RouteCaseCommand(0, new RouteCaseRequest(1));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_WhenRequestIsNull_Fails()
    {
        var command = new RouteCaseCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }

    [Fact]
    public async Task Validate_WhenIdAndRequestAreInvalid_Fails()
    {
        var command = new RouteCaseCommand(0, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.CaseRequest);
    }
}


