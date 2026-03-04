using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public class DeleteAccountContactRoleCommandValidatorTest
{
    private readonly DeleteAccountContactRoleCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new DeleteAccountContactRoleCommand(1, new DeleteAccountContactRoleRequest(1)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new DeleteAccountContactRoleCommand(0, new DeleteAccountContactRoleRequest(0)));
        Assert.False(result.IsValid);
    }
}
