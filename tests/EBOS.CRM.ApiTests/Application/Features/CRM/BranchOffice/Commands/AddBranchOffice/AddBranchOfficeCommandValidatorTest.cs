using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public class AddBranchOfficeCommandValidatorTest
{
    private readonly AddBranchOfficeCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new AddBranchOfficeCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyName_Fails(string value)
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest() with { Name = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyPhoneNumber_Fails(string value)
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest() with { PhoneNumber = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.PhoneNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidCorporateCustomerId_Fails(long value)
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest() with { CorporateCustomerId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.CorporateCustomerId);
    }

    private static AddBranchOfficeRequest BuildAddRequest() => new(
            TenantId: 1,
            Name: "Main",
            PhoneNumber: "123",
            CorporateCustomerId: 1
        );
}


