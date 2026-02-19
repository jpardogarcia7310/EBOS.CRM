using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;

public class UpdateCorporateCustomerCommandValidatorTest
{
    private readonly UpdateCorporateCustomerCommandValidator _validator = CreateValidator();

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

    private static UpdateCorporateCustomerCommandValidator CreateValidator()
    {
        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Country>
            {
                new() { Id = 1, Iso31661A2Code = "EC", Name = "Ecuador", CreatedAt = DateTime.UtcNow, CreatedBy = 1, Currency = "USD", CurrencyCode = "USD", Domain = ".ec", InternationalPhoneCode = "593", Iso31661A3Code = "ECU", Iso31661NumCode = "218" }
            });

        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new UpdateCorporateCustomerCommandValidator(countryRepo.Object, validationCatalog.Object);
    }
}


