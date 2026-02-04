using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public class AuthenticateUserCommandValidatorTest
{
    private readonly AuthenticateUserCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AuthenticateUserCommand(BuildRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyExternalId_Fails()
    {
        var command = new AuthenticateUserCommand(BuildRequest() with { ExternalId = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.ExternalId);
    }

    [Fact]
    public void Validate_InvalidEmail_Fails()
    {
        var command = new AuthenticateUserCommand(BuildRequest() with { Email = "not-an-email" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    [Fact]
    public void Validate_EmptyDisplayName_Fails()
    {
        var command = new AuthenticateUserCommand(BuildRequest() with { DisplayName = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.DisplayName);
    }

    private static AuthenticateUserRequest BuildRequest() => new(
        ExternalId: "ext-1",
        Username: "jdoe",
        Email: "jdoe@example.com",
        DisplayName: "John Doe",
        IsActive: true);
}
