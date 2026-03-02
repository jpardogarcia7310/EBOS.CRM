using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.AddBankInformation;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.AddBranchOfficeAddress;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;
using EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.AddIndividualCustomer;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.AddTaxInformationAddress;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentAssertions;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Validators;

public class CrmCoreValidatorTests
{
    [Fact]
    public async Task AddAddress_Validates_All_Fields()
    {
        var validator = CreateAddressValidator();
        var request = new AddAddressRequest(1, "Street", "1", "A", "B", "C", "N", "City",
            "State", "12345", "https://maps.test", "10", "-20", 1, 1);
        (await validator.ValidateAsync(new AddAddressCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddAddressCommand(request with { Street = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { ExternalNumber = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { City = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { StateOrProvince = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { PostalCode = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { CountryId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { AddressTypeId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { GoogleMapsUrl = "http://invalid" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { Latitude = "999" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { Longitude = "999" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { Street = Long(201) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { ExternalNumber = Long(21) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { InternalNumber = Long(21) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { BetweenStreet1 = Long(201) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { BetweenStreet2 = Long(201) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { Neighbourhood = Long(201) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { City = Long(151) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { StateOrProvince = Long(151) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { PostalCode = Long(21) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddAddressCommand(request with { GoogleMapsUrl = Long(501) }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddCustomer_Validates_All_Fields()
    {
        var validator = CreateCustomerValidator();
        var request = new AddCustomerRequest(1, "CUST", "a@b.com", "123456", 1);
        (await validator.ValidateAsync(new AddCustomerCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddCustomerCommand(request with { Code = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCustomerCommand(request with { Email = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCustomerCommand(request with { Phone = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCustomerCommand(request with { StatusId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddCorporateCustomer_Validates_All_Fields()
    {
        var validator = CreateCorporateValidator();
        var request = new AddCorporateCustomerRequest(1, "C001", "corp@b.com", "1111", 1, "Acme", "TAX123");
        (await validator.ValidateAsync(new AddCorporateCustomerCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddCorporateCustomerCommand(request with { LegalName = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCorporateCustomerCommand(request with { TaxIdentification = "" }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddIndividualCustomer_Validates_All_Fields()
    {
        var validator = CreateIndividualValidator();
        var request = new AddIndividualCustomerRequest(1, "I001", "ind@b.com", "1111", 1, "Jane", "Doe",
            DateTime.UtcNow.Date, "1234567890", 1);
        (await validator.ValidateAsync(new AddIndividualCustomerCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddIndividualCustomerCommand(request with { FirstName = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddIndividualCustomerCommand(request with { LastName = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddIndividualCustomerCommand(request with { IdentificationTypeId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddIndividualCustomerCommand(request with { IdentificationNumber = Long(501) }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddCustomerAddress_Validates_All_Fields()
    {
        var validator = new AddCustomerAddressCommandValidator();
        var request = new AddCustomerAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        (await validator.ValidateAsync(new AddCustomerAddressCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddCustomerAddressCommand(request with { CustomerId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCustomerAddressCommand(request with { AddressId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddBranchOffice_Validates_All_Fields()
    {
        var validator = new AddBranchOfficeCommandValidator();
        var request = new AddBranchOfficeRequest(1, "HQ", "1111", 2);
        (await validator.ValidateAsync(new AddBranchOfficeCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddBranchOfficeCommand(request with { Name = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddBranchOfficeCommand(request with { PhoneNumber = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddBranchOfficeCommand(request with { CorporateCustomerId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddBranchOfficeAddress_Validates_All_Fields()
    {
        var validator = new AddBranchOfficeAddressCommandValidator();
        var request = new AddBranchOfficeAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        (await validator.ValidateAsync(new AddBranchOfficeAddressCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddBranchOfficeAddressCommand(request with { BranchOfficeId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddBranchOfficeAddressCommand(request with { AddressId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddBankInformation_Validates_All_Fields()
    {
        var validator = new AddBankInformationCommandValidator();
        var request = new AddBankInformationRequest(1, "IBAN", "BIC", "Bank", 2);
        (await validator.ValidateAsync(new AddBankInformationCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddBankInformationCommand(request with { Iban = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddBankInformationCommand(request with { Bic = Long(501) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddBankInformationCommand(request with { BankName = Long(501) }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddBankInformationCommand(request with { CustomerId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddTaxInformation_Validates_All_Fields()
    {
        var validator = new AddTaxInformationCommandValidator();
        var request = new AddTaxInformationRequest(1, "Tax Name", "TIN123", 2);
        (await validator.ValidateAsync(new AddTaxInformationCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddTaxInformationCommand(request with { TaxName = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddTaxInformationCommand(request with { TaxIdentificationNumber = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddTaxInformationCommand(request with { CustomerId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddTaxInformationAddress_Validates_All_Fields()
    {
        var validator = new AddTaxInformationAddressCommandValidator();
        var request = new AddTaxInformationAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        (await validator.ValidateAsync(new AddTaxInformationAddressCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddTaxInformationAddressCommand(request with { TaxInformationId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddTaxInformationAddressCommand(request with { AddressId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddCreditAccount_Validates_All_Fields()
    {
        var validator = new AddCreditAccountCommandValidator();
        var request = new AddCreditAccountRequest(1, 1000m, 100m, 2);
        (await validator.ValidateAsync(new AddCreditAccountCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddCreditAccountCommand(request with { CustomerId = 0 }))).IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task AddCreditTransaction_Validates_All_Fields()
    {
        var validator = new AddCreditTransactionCommandValidator();
        var request = new AddCreditTransactionRequest(1, DateTime.UtcNow.Date, 100m, "Consumo", "EXT", "Ok", 2);
        (await validator.ValidateAsync(new AddCreditTransactionCommand(request))).IsValid.Should().BeTrue();

        (await validator.ValidateAsync(new AddCreditTransactionCommand(request with { CreditAccountId = 0 }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCreditTransactionCommand(request with { Type = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCreditTransactionCommand(request with { ExternalReference = "" }))).IsValid.Should().BeFalse();
        (await validator.ValidateAsync(new AddCreditTransactionCommand(request with { Comments = "" }))).IsValid.Should().BeFalse();
    }

    private static string Long(int length) => new('X', length);

    private static AddAddressCommandValidator CreateAddressValidator()
    {
        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Country { Id = 1, Iso31661A2Code = "EC", Name = "Ecuador", CreatedAt = DateTime.UtcNow, CreatedBy = 1, Currency = "USD", CurrencyCode = "USD", Domain = ".ec", InternationalPhoneCode = "593", Iso31661A3Code = "ECU", Iso31661NumCode = "218" });

        var addressTypeRepo = new Mock<IAddressTypeRepository>();
        addressTypeRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AddressType { Id = 1, Code = "HOME", Description = "Home", CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = null, UpdatedBy = null });

        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new AddAddressCommandValidator(countryRepo.Object, addressTypeRepo.Object, validationCatalog.Object);
    }

    private static AddCorporateCustomerCommandValidator CreateCorporateValidator()
    {
        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Country
            {
                Id = 1,
                Iso31661A2Code = "EC",
                Name = "Ecuador",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                Currency = "USD",
                CurrencyCode = "USD",
                Domain = ".ec",
                InternationalPhoneCode = "593",
                Iso31661A3Code = "ECU",
                Iso31661NumCode = "218"
            });

        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new AddCorporateCustomerCommandValidator(countryRepo.Object, validationCatalog.Object);
    }

    private static AddIndividualCustomerCommandValidator CreateIndividualValidator()
    {
        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Country
            {
                Id = 1,
                Iso31661A2Code = "EC",
                Name = "Ecuador",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                Currency = "USD",
                CurrencyCode = "USD",
                Domain = ".ec",
                InternationalPhoneCode = "593",
                Iso31661A3Code = "ECU",
                Iso31661NumCode = "218"
            });

        var identificationRepo = new Mock<IIdentificationTypeRepository>();
        identificationRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentificationType { Id = 1, Code = "DNI", Description = "DNI", CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = null, UpdatedBy = null, Erased = false });

        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new AddIndividualCustomerCommandValidator(countryRepo.Object, identificationRepo.Object, validationCatalog.Object);
    }

    private static AddCustomerCommandValidator CreateCustomerValidator()
    {
        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?)null);

        return new AddCustomerCommandValidator(validationCatalog.Object, countryRepo.Object);
    }
}



