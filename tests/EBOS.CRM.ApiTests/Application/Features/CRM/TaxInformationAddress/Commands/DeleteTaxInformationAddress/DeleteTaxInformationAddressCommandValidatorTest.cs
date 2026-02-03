using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.DeleteTaxInformationAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Commands.DeleteTaxInformationAddress;

public class DeleteTaxInformationAddressCommandValidatorTest
{
    private readonly DeleteTaxInformationAddressCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteTaxInformationAddressCommand(id);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
