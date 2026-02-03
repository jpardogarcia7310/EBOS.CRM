using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.AddBankInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Commands.AddBankInformation;

public class AddBankInformationCommandValidatorTest
{
    private readonly AddBankInformationCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddBankInformationCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddBankInformationRequest BuildAddRequest() => new(
            Iban: "ES1200000000000000000000",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: 1
        );
}
