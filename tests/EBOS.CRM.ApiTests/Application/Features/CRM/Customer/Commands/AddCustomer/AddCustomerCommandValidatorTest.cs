using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandValidatorTest
{
    private readonly AddCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddCustomerCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new AddCustomerCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyCode_Fails(string value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { Code = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyEmail_Fails(string value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { Email = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyPhone_Fails(string value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { Phone = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidStatus_Fails(long value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { StatusId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.StatusId);
    }

    private static AddCustomerRequest BuildAddRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1
        );
}


