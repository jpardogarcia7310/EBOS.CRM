using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;

public class GetAccountContactsByAccountQueryValidatorTest
{
    private readonly GetAccountContactsByAccountQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAccountContactsByAccountQuery(1, 2, 1, 20));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidPaging_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAccountContactsByAccountQuery(0, 0, 0, 0));
        Assert.False(result.IsValid);
    }
}
