using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;

public class AddAccountHierarchyCommandValidatorTest
{
    private readonly AddAccountHierarchyCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(
            1, 10, 20, "HOLDING", DateTime.UtcNow));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_SameParentAndChild_Fails()
    {
        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(
            1, 10, 10, "HOLDING", DateTime.UtcNow));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AccountHierarchyRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidTenantId_Fails(long tenantId)
    {
        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(
            tenantId, 10, 20, "HOLDING", DateTime.UtcNow));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AccountHierarchyRequest.TenantId);
    }
}


