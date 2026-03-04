using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;

public class AddAccountContactRoleCommandValidatorTest
{
    private readonly AddAccountContactRoleCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var request = new AddAccountContactRoleRequest(1, 10, "OWNER", true, DateTime.UtcNow, null);
        var result = await _validator.TestValidateAsync(new AddAccountContactRoleCommand(request));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidDateWindow_Fails()
    {
        var start = DateTime.UtcNow;
        var request = new AddAccountContactRoleRequest(1, 10, "OWNER", true, start, start.AddDays(-1));
        var result = await _validator.TestValidateAsync(new AddAccountContactRoleCommand(request));
        Assert.False(result.IsValid);
    }
}
