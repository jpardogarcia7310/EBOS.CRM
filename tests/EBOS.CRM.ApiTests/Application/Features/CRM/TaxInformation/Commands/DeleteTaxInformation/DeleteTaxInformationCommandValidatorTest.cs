using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.DeleteTaxInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Commands.DeleteTaxInformation;

public class DeleteTaxInformationCommandValidatorTest
{
    private readonly DeleteTaxInformationCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteTaxInformationCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




