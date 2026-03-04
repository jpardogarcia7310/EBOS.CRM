using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;

public class GetAccountHierarchyByAccountQueryValidatorTest
{
    private readonly GetAccountHierarchyByAccountQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAccountHierarchyByAccountQuery(1, 10, 1, 10));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAccountHierarchyByAccountQuery(0, 0, 0, 0));
        Assert.False(result.IsValid);
    }
}
