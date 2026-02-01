using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;

public class AddTaxInformationCommandValidatorTest
{
    private readonly AddTaxInformationCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddTaxInformationCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddTaxInformationRequest BuildAddRequest() => new(
            TaxName: "Tax",
            TaxIdentificationNumber: "TAX123",
            CustomerId: 1
        );
}