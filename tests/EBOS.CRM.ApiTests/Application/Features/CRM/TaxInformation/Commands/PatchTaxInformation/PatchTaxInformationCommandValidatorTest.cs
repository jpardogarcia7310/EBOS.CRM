using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;

public class PatchTaxInformationCommandValidatorTest
{
    private readonly PatchTaxInformationCommandValidator _validator = new();

    [Fact]
    public void Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchTaxInformationRequest(
            TenantId: 1,
            TaxName: null,
            TaxIdentificationNumber: null,
            CustomerId: null);
        var command = new PatchTaxInformationCommand(1, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TaxInformationRequest)
            .WithErrorMessage("At least one field must be provided.");
    }
}
