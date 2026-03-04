using EBOS.CRM.Application.Features.CRM.Quote.Queries.GetQuoteById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Queries.GetQuoteById;

public class GetQuoteByIdQueryValidatorTest
{
    private readonly GetQuoteByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetQuoteByIdQuery(1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetQuoteByIdQuery(0));
        Assert.False(result.IsValid);
    }
}
