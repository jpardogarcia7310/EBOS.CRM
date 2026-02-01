using EBOS.CRM.Application.Features.CRM.Customer.Commands.DeleteCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.DeleteCustomer;

public class DeleteCustomerCommandValidatorTest
{
    private readonly DeleteCustomerCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteCustomerCommand(id);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}