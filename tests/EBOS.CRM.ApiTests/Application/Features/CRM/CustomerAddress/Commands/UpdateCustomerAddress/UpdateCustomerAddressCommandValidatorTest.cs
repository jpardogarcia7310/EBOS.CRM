using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.UpdateCustomerAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerAddress.Commands.UpdateCustomerAddress;

public class UpdateCustomerAddressCommandValidatorTest
{
    private readonly UpdateCustomerAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCustomerAddressCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

private static UpdateCustomerAddressRequest BuildUpdateRequest() => new(
        CustomerId: 1,
        AddressId: 1,
        IsPrimary: true,
        ValidFrom: DateTime.UtcNow,
        ValidTo: null,
        IsCurrent: true
    );
}