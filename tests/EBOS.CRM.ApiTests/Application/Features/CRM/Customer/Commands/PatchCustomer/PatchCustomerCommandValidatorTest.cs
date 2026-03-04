using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandValidatorTest
{
    private readonly PatchCustomerCommandValidator _validator = CreateValidator();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var request = BuildRequest();
        var command = new PatchCustomerCommand(0, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var command = new PatchCustomerCommand(1, null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest);
    }

    [Fact]
    public async Task Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchCustomerRequest(
            TenantId: 1,
            Code: null,
            Email: null,
            Phone: null,
            StatusId: null);
        var command = new PatchCustomerCommand(1, request);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest)
            .WithErrorMessage("At least one field must be provided.");
    }

    [Fact]
    public async Task Validate_EmptyCode_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Code = "" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Fact]
    public async Task Validate_CodeTooLong_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Code = new string('a', 51) });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Code);
    }

    [Fact]
    public async Task Validate_EmptyEmail_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Email = "" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Fact]
    public async Task Validate_EmailTooLong_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Email = new string('a', 101) });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Email = "bad-email" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Email);
    }

    [Fact]
    public async Task Validate_EmptyPhone_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Phone = "" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Fact]
    public async Task Validate_PhoneTooLong_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Phone = new string('1', 13) });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Fact]
    public async Task Validate_PhoneNonDigits_Fails()
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { Phone = "123-abc" });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.Phone);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidStatus_Fails(long value)
    {
        var command = new PatchCustomerCommand(1, BuildRequest() with { StatusId = value });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerRequest.StatusId!.Value);
    }

    private static PatchCustomerRequest BuildRequest() => new(
        TenantId: 1,
        Code: "C001",
        Email: "a@b.com",
        Phone: "123",
        StatusId: 1);

    private static PatchCustomerCommandValidator CreateValidator()
    {
        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var countryRepository = new Mock<ICountryRepository>();
        countryRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country)null!);

        return new PatchCustomerCommandValidator(validationCatalog.Object, countryRepository.Object);
    }
}


