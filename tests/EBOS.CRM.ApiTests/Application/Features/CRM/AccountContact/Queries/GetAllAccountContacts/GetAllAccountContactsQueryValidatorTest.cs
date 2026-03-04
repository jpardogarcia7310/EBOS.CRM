using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;

public class GetAllAccountContactsQueryValidatorTest
{
    private readonly GetAllAccountContactsQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllAccountContactsQuery(1, 1, 20));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllAccountContactsQuery(0, 0, 0));
        Assert.False(result.IsValid);
    }
}
