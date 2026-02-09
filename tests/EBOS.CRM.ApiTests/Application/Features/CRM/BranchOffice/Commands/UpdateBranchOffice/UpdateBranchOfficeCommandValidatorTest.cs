using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;

public class UpdateBranchOfficeCommandValidatorTest
{
    private readonly UpdateBranchOfficeCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateBranchOfficeCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new UpdateBranchOfficeCommand(1, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new UpdateBranchOfficeCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyName_Fails(string value)
    {
        var command = new UpdateBranchOfficeCommand(1, BuildUpdateRequest() with { Name = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyPhoneNumber_Fails(string value)
    {
        var command = new UpdateBranchOfficeCommand(1, BuildUpdateRequest() with { PhoneNumber = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.PhoneNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidCorporateCustomerId_Fails(long value)
    {
        var command = new UpdateBranchOfficeCommand(1, BuildUpdateRequest() with { CorporateCustomerId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest.CorporateCustomerId);
    }

    private static UpdateBranchOfficeRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Id: 1,
            Name: "Main",
            PhoneNumber: "123",
            CorporateCustomerId: 1
        );
}


