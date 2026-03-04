using EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Commands.AddQuote;

public class AddQuoteCommandValidatorTest
{
    private readonly AddQuoteCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddQuoteRequest(1, 10, "Draft", "Q-1", 100m, 10m, 90m, DateTime.UtcNow.AddDays(7), "notes");
        var result = await _validator.TestValidateAsync(new AddQuoteCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddQuoteCommand(null!));
        Assert.False(result.IsValid);
    }
}
