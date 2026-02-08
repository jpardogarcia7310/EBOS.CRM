using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;

public class UpdateCorporateCustomerCommandValidatorTest
{
    private readonly UpdateCorporateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCorporateCustomerCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new UpdateCorporateCustomerCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyCode_Fails(string value)
    {
        var command = new UpdateCorporateCustomerCommand(1, BuildUpdateRequest() with { Code = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyEmail_Fails(string value)
    {
        var command = new UpdateCorporateCustomerCommand(1, BuildUpdateRequest() with { Email = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyPhone_Fails(string value)
    {
        var command = new UpdateCorporateCustomerCommand(1, BuildUpdateRequest() with { Phone = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest.Phone);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyLegalName_Fails(string value)
    {
        var command = new UpdateCorporateCustomerCommand(1, BuildUpdateRequest() with { LegalName = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest.LegalName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyTaxIdentification_Fails(string value)
    {
        var command = new UpdateCorporateCustomerCommand(1, BuildUpdateRequest() with { TaxIdentification = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest.TaxIdentification);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidStatus_Fails(long value)
    {
        var command = new UpdateCorporateCustomerCommand(1, BuildUpdateRequest() with { StatusId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CorporateCustomerRequest.StatusId);
    }

    private static UpdateCorporateCustomerRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1,
            LegalName: "Corp",
            TaxIdentification: "TAX999"
        );
}


