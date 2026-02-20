using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidatorTest
{
    private readonly UpdateCustomerCommandValidator _validator = CreateValidator();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCustomerCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new UpdateCustomerCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyCode_Fails(string value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { Code = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyEmail_Fails(string value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { Email = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyPhone_Fails(string value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { Phone = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidStatus_Fails(long value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { StatusId = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.StatusId);
    }

    private static UpdateCustomerRequest BuildUpdateRequest() => new(
            Id: 1,
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1
        );

    private static UpdateCustomerCommandValidator CreateValidator()
    {
        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new UpdateCustomerCommandValidator(validationCatalog.Object);
    }
}



