using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
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

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new AddBankInformationCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyIban_Fails(string value)
    {
        var command = new AddBankInformationCommand(BuildAddRequest() with { Iban = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.Iban);
    }

    [Fact]
    public void Validate_BicTooLong_Fails()
    {
        var command = new AddBankInformationCommand(BuildAddRequest() with { Bic = new string('a', 501) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.Bic);
    }

    [Fact]
    public void Validate_BankNameTooLong_Fails()
    {
        var command = new AddBankInformationCommand(BuildAddRequest() with { BankName = new string('a', 501) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.BankName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidCustomerId_Fails(long value)
    {
        var command = new AddBankInformationCommand(BuildAddRequest() with { CustomerId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.CustomerId);
    }

    private static AddBankInformationRequest BuildAddRequest() => new(
            TenantId: 1,
            Iban: "ES1200000000000000000000",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: 1
        );
}


