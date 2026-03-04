using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;

public class UpdateBankInformationCommandValidatorTest
{
    private readonly UpdateBankInformationCommandValidator _validator = new();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateBankInformationCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new UpdateBankInformationCommand(1, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var command = new UpdateBankInformationCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyIban_Fails(string value)
    {
        var command = new UpdateBankInformationCommand(1, BuildUpdateRequest() with { Iban = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.Iban);
    }

    [Fact]
    public async Task Validate_BicTooLong_Fails()
    {
        var command = new UpdateBankInformationCommand(1, BuildUpdateRequest() with { Bic = new string('a', 501) });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.Bic);
    }

    [Fact]
    public async Task Validate_BankNameTooLong_Fails()
    {
        var command = new UpdateBankInformationCommand(1, BuildUpdateRequest() with { BankName = new string('a', 501) });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.BankName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidCustomerId_Fails(long value)
    {
        var command = new UpdateBankInformationCommand(1, BuildUpdateRequest() with { CustomerId = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BankInformationRequest.CustomerId);
    }

    private static UpdateBankInformationRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Iban: "ES1200000000000000000000",
            Bic: "BANKESMM",
            BankName: "Bank",
            CustomerId: 1
        );
}




