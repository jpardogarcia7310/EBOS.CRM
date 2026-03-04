using EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;

public class GetAccountContactRoleByIdQueryValidatorTest
{
    private readonly GetAccountContactRoleByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAccountContactRoleByIdQuery(1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ZeroId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAccountContactRoleByIdQuery(0));
        Assert.False(result.IsValid);
    }
}
