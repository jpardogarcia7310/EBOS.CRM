using EBOS.CRM.Application.Features.CRM.Quote.Commands.UpdateQuote;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Commands.UpdateQuote;

public class UpdateQuoteCommandValidatorTest
{
    private readonly UpdateQuoteCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var req = new UpdateQuoteRequest(1, 1, 10, "Draft", "Q-1", 100m, 10m, 90m, null, null);
        var result = await _validator.TestValidateAsync(new UpdateQuoteCommand(1, req));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new UpdateQuoteCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
