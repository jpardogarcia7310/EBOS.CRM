using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public class AuthorizeUserQueryValidatorTest
{
    private readonly AuthorizeUserQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var query = new AuthorizeUserQuery(BuildRequest());

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidUserId_Fails()
    {
        var query = new AuthorizeUserQuery(BuildRequest() with { UserId = 0 });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.UserId);
    }

    [Fact]
    public void Validate_EmptyPolicyCode_Fails()
    {
        var query = new AuthorizeUserQuery(BuildRequest() with { PolicyCode = "" });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.PolicyCode);
    }

    private static AuthorizeUserRequest BuildRequest() => new(
        UserId: 1,
        PolicyCode: "crm.customer.access");
}
