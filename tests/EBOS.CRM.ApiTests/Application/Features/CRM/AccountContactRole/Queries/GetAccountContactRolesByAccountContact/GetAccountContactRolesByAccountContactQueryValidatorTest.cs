using EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;

public class GetAccountContactRolesByAccountContactQueryValidatorTest
{
    private readonly GetAccountContactRolesByAccountContactQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new GetAccountContactRolesByAccountContactQuery(1, 10, 1, 20));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new GetAccountContactRolesByAccountContactQuery(0, 0, 0, 0));
        Assert.False(result.IsValid);
    }
}
