using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandValidatorTest
{
    private readonly PatchCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchCustomerRequest(
            TenantId: 1,
            Code: null,
            Email: null,
            Phone: null,
            StatusId: null);
        var command = new PatchCustomerCommand(1, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest)
            .WithErrorMessage("At least one field must be provided.");
    }
}
