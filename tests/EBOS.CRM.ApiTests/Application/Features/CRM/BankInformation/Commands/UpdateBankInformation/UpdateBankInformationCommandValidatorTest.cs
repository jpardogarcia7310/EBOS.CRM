using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;

public class UpdateBankInformationCommandValidatorTest
{
    private readonly UpdateBankInformationCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateBankInformationCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateBankInformationRequest BuildUpdateRequest() => new(
            Iban: "ES1200000000000000000000",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: 1
        );
}