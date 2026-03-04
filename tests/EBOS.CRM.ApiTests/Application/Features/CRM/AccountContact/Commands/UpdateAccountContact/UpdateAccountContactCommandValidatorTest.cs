using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.UpdateAccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.UpdateAccountContact;

public class UpdateAccountContactCommandValidatorTest
{
    private readonly UpdateAccountContactCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new UpdateAccountContactRequest(1, 2, 3, false, DateTime.UtcNow, null);
        var result = await _validator.TestValidateAsync(new UpdateAccountContactCommand(1, request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EndAtBeforeStartAt_Fails()
    {
        var startAt = DateTime.UtcNow;
        var request = new UpdateAccountContactRequest(1, 2, 3, false, startAt, startAt.AddDays(-1));
        var result = await _validator.TestValidateAsync(new UpdateAccountContactCommand(1, request));
        Assert.False(result.IsValid);
    }
}
