using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;

public class AddAccountHierarchyCommandValidatorTest
{
    private readonly AddAccountHierarchyCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(
            1, 10, 20, "HOLDING", DateTime.UtcNow));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_SameParentAndChild_Fails()
    {
        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(
            1, 10, 10, "HOLDING", DateTime.UtcNow));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AccountHierarchyRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidTenantId_Fails(long tenantId)
    {
        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(
            tenantId, 10, 20, "HOLDING", DateTime.UtcNow));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AccountHierarchyRequest.TenantId);
    }
}
