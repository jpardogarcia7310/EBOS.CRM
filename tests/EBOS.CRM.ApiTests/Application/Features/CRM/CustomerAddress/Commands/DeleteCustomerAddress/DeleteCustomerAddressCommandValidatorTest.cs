using EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.DeleteCustomerAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerAddress.Commands.DeleteCustomerAddress;

public class DeleteCustomerAddressCommandValidatorTest
{
    private readonly DeleteCustomerAddressCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteCustomerAddressCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




