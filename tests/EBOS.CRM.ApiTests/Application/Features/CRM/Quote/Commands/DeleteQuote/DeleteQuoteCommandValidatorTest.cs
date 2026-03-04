using EBOS.CRM.Application.Features.CRM.Quote.Commands.DeleteQuote;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Commands.DeleteQuote;

public class DeleteQuoteCommandValidatorTest
{
    private readonly DeleteQuoteCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidId_Passes()
    {
        var result = await _validator.TestValidateAsync(new DeleteQuoteCommand(1));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(new DeleteQuoteCommand(0));
        Assert.False(result.IsValid);
    }
}
