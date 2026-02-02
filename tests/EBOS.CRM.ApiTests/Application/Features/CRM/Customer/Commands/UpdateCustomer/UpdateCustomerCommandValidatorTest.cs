using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidatorTest
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCustomerCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateCustomerRequest BuildUpdateRequest() => new(
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            CreatedAt: DateTime.UtcNow,
            StatusId: 1
        );
}