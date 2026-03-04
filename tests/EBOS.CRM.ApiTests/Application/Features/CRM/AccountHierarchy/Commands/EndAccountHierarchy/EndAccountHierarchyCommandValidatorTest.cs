using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;

public class EndAccountHierarchyCommandValidatorTest
{
    private readonly EndAccountHierarchyCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new EndAccountHierarchyCommand(1, new EndAccountHierarchyRequest(1, DateTime.UtcNow)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new EndAccountHierarchyCommand(0, new EndAccountHierarchyRequest(0, default)));
        Assert.False(result.IsValid);
    }
}
