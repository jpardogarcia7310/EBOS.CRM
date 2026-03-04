using EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public class GetAllQuotesQueryValidatorTest
{
    private readonly GetAllQuotesQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllQuotesQuery(1, 10));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllQuotesQuery(0, 10));
        Assert.False(result.IsValid);
    }
}
