using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandValidatorTest
{
    private readonly PatchCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var request = BuildRequest();
        var command = new PatchCustomerCommand(0, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new PatchCustomerCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest);
    }

    [Fact]
    public void Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchCustomerRequest(
            TenantId: 1,
            Code: null,
            Email: null,
            Phone: null,
            StatusId: null);
        var command = new PatchCustomerCommand(1, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest)
            .WithErrorMessage("At least one field must be provided.");
    }

    [Fact]
    public void Validate_EmptyCode_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Code = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Fact]
    public void Validate_CodeTooLong_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Code = new string('a', 51) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Fact]
    public void Validate_EmptyEmail_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Email = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Fact]
    public void Validate_EmailTooLong_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Email = new string('a', 101) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Fact]
    public void Validate_InvalidEmailFormat_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Email = "bad-email" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Fact]
    public void Validate_EmptyPhone_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Phone = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Fact]
    public void Validate_PhoneTooLong_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Phone = new string('1', 13) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Fact]
    public void Validate_PhoneNonDigits_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Phone = "123-abc" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidStatus_Fails(long value)
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { StatusId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.StatusId!.Value);
    }

    private static PatchCustomerRequest BuildRequest() => new(
        TenantId: 1,
        Code: "C001",
        Email: "a@b.com",
        Phone: "123",
        StatusId: 1);
}
