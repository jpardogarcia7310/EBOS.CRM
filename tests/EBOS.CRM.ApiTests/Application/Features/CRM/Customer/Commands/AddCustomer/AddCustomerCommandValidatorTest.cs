using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandValidatorTest
{
    private readonly AddCustomerCommandValidator _validator = CreateValidator();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AddCustomerCommand(BuildAddRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var command = new AddCustomerCommand(null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyCode_Fails(string value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { Code = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyEmail_Fails(string value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { Email = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyPhone_Fails(string value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { Phone = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidStatus_Fails(long value)
    {
        var command = new AddCustomerCommand(BuildAddRequest() with { StatusId = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.StatusId);
    }

    private static AddCustomerRequest BuildAddRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1
        );

    private static AddCustomerCommandValidator CreateValidator()
    {
        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var countryRepository = new Mock<ICountryRepository>();
        countryRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country)null!);

        return new AddCustomerCommandValidator(validationCatalog.Object, countryRepository.Object);
    }
}




