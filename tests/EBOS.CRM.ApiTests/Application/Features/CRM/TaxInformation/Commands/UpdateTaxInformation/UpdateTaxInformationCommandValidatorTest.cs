using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public class UpdateTaxInformationCommandValidatorTest
{
    private readonly UpdateTaxInformationCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateTaxInformationCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateTaxInformationRequest BuildUpdateRequest() => new(
            TaxName: "Tax",
            TaxIdentificationNumber: "TAX123",
            CustomerId: 1
        );
}
