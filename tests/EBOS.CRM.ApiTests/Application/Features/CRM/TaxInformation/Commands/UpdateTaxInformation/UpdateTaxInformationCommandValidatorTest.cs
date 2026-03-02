using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public class UpdateTaxInformationCommandValidatorTest
{
    private readonly UpdateTaxInformationCommandValidator _validator = new();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateTaxInformationCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateTaxInformationRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Id: 1,
            TaxName: "Tax",
            TaxIdentificationNumber: "TAX123",
            CustomerId: 1
        );
}




