using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.AddAccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.AddAccountContact;

public class AddAccountContactCommandValidatorTest
{
    private readonly AddAccountContactCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddAccountContactRequest(1, 2, 3, true, DateTime.UtcNow, null);
        var result = await _validator.TestValidateAsync(new AddAccountContactCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EndAtBeforeStartAt_Fails()
    {
        var startAt = DateTime.UtcNow;
        var request = new AddAccountContactRequest(1, 2, 3, true, startAt, startAt.AddDays(-1));
        var result = await _validator.TestValidateAsync(new AddAccountContactCommand(request));
        Assert.False(result.IsValid);
    }
}
