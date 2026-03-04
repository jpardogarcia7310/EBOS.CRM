using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public class PatchBranchOfficeCommandValidatorTest
{
    private readonly PatchBranchOfficeCommandValidator _validator = new();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: "Main",
            PhoneNumber: "123",
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(0, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var command = new PatchBranchOfficeCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest);
    }

    [Fact]
    public async Task Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: null,
            PhoneNumber: null,
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest)
            .WithErrorMessage("At least one field must be provided.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyName_Fails(string value)
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: value,
            PhoneNumber: null,
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.Name);
    }

    [Fact]
    public async Task Validate_NameTooLong_Fails()
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: new string('a', 201),
            PhoneNumber: null,
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyPhoneNumber_Fails(string value)
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: null,
            PhoneNumber: value,
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.PhoneNumber);
    }

    [Fact]
    public async Task Validate_PhoneNumberTooLong_Fails()
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: null,
            PhoneNumber: new string('a', 21),
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.PhoneNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidCorporateCustomerId_Fails(long value)
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: null,
            PhoneNumber: null,
            CorporateCustomerId: value);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.CorporateCustomerId!.Value);
    }
}


