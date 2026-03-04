using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public class DeleteAccountContactCommandValidatorTest
{
    private readonly DeleteAccountContactCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new DeleteAccountContactCommand(10, new DeleteAccountContactRequest(1)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidIds_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new DeleteAccountContactCommand(0, new DeleteAccountContactRequest(0)));
        Assert.False(result.IsValid);
    }
}
