using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;

public class GetAccountHierarchyByIdQueryValidatorTest
{
    private readonly GetAccountHierarchyByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAccountHierarchyByIdQuery(1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ZeroId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAccountHierarchyByIdQuery(0));
        Assert.False(result.IsValid);
    }
}
