using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;

public class GetAccountContactByIdQueryValidatorTest
{
    private readonly GetAccountContactByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAccountContactByIdQuery(1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NonPositiveId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAccountContactByIdQuery(0));
        Assert.False(result.IsValid);
    }
}
