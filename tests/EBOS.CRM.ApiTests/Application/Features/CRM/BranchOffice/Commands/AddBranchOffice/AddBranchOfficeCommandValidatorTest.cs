using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public class AddBranchOfficeCommandValidatorTest
{
    private readonly AddBranchOfficeCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var command = new AddBranchOfficeCommand(null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyName_Fails(string value)
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest() with { Name = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyPhoneNumber_Fails(string value)
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest() with { PhoneNumber = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.PhoneNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidCorporateCustomerId_Fails(long value)
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest() with { CorporateCustomerId = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.CorporateCustomerId);
    }

    private static AddBranchOfficeRequest BuildAddRequest() => new(
            TenantId: 1,
            Name: "Main",
            PhoneNumber: "123",
            CorporateCustomerId: 1
        );
}




