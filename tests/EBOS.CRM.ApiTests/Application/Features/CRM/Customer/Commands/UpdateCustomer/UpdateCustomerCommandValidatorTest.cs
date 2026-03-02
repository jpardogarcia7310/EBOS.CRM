using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidatorTest
{
    private readonly UpdateCustomerCommandValidator _validator = CreateValidator();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateCustomerCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var command = new UpdateCustomerCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyCode_Fails(string value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { Code = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyEmail_Fails(string value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { Email = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyPhone_Fails(string value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { Phone = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidStatus_Fails(long value)
    {
        var command = new UpdateCustomerCommand(1, BuildUpdateRequest() with { StatusId = value });

        var result = await _validator.TestValidateAsync(command);

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

        var countryRepository = new Mock<ICountryRepository>();
        countryRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country)null!);

        return new UpdateCustomerCommandValidator(validationCatalog.Object, countryRepository.Object);
    }
}





