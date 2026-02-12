using EBOS.CRM.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;

public class UpdateTaxInformationAddressCommandValidatorTest
{
    private readonly UpdateTaxInformationAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateTaxInformationAddressCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateTaxInformationAddressRequest BuildUpdateRequest() => new(
            TenantId: 1,
            TaxInformationId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow,
            ValidTo: null,
            IsCurrent: true
        );
}


