using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.UpdateAccountContactRole;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Commands.UpdateAccountContactRole;

public class UpdateAccountContactRoleCommandValidatorTest
{
    private readonly UpdateAccountContactRoleCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new UpdateAccountContactRoleRequest(1, 10, "OWNER", false, DateTime.UtcNow, null);
        var result = await _validator.TestValidateAsync(new UpdateAccountContactRoleCommand(1, request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_PrimaryInactive_Fails()
    {
        var request = new UpdateAccountContactRoleRequest(1, 10, "OWNER", true, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        var result = await _validator.TestValidateAsync(new UpdateAccountContactRoleCommand(1, request));
        Assert.False(result.IsValid);
    }
}
