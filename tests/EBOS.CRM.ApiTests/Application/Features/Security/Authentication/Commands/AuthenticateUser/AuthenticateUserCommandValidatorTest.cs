using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public class AuthenticateUserCommandValidatorTest
{
    private readonly AuthenticateUserCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AuthenticateUserCommand(BuildRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyExternalId_Fails()
    {
        var command = new AuthenticateUserCommand(BuildRequest() with { ExternalId = "" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.ExternalId);
    }

    [Fact]
    public async Task Validate_InvalidEmail_Fails()
    {
        var command = new AuthenticateUserCommand(BuildRequest() with { Email = "not-an-email" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    [Fact]
    public async Task Validate_EmptyDisplayName_Fails()
    {
        var command = new AuthenticateUserCommand(BuildRequest() with { DisplayName = "" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.DisplayName);
    }

    private static AuthenticateUserRequest BuildRequest() => new(
        ExternalId: "ext-1",
        Username: "jdoe",
        Email: "jdoe@example.com",
        DisplayName: "John Doe",
        IsActive: true);
}


