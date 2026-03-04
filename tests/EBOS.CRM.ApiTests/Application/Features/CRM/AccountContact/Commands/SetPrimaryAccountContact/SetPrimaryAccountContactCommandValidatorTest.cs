using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;

public class SetPrimaryAccountContactCommandValidatorTest
{
    private readonly SetPrimaryAccountContactCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new SetPrimaryAccountContactCommand(1, new SetPrimaryAccountContactRequest(1, true)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidIdOrTenant_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new SetPrimaryAccountContactCommand(0, new SetPrimaryAccountContactRequest(0, true)));
        Assert.False(result.IsValid);
    }
}
