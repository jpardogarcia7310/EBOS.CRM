using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.DeleteBankInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Commands.DeleteBankInformation;

public class DeleteBankInformationCommandValidatorTest
{
    private readonly DeleteBankInformationCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteBankInformationCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




