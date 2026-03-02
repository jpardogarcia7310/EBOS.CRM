using EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;

public class AddCustomerAddressCommandValidatorTest
{
    private readonly AddCustomerAddressCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AddCustomerAddressCommand(BuildAddRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddCustomerAddressRequest BuildAddRequest() => new(
            TenantId: 1,
            CustomerId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow,
            ValidTo: null,
            IsCurrent: true
        );
}




