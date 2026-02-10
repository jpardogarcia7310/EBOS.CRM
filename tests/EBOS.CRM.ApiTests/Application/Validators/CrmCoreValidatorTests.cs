using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;
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
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Application.Validators;

public class CrmCoreValidatorTests
{
    [Fact]
    public void AddAddress_Validates_All_Fields()
    {
        var validator = new AddAddressCommandValidator();
        var request = new AddAddressRequest(1, "Street", "1", "A", "B", "C", "N", "City",
            "State", "12345", "https://maps.test", "10", "-20", 1, 1);
        validator.Validate(new AddAddressCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddAddressCommand(request with { Street = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { ExternalNumber = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { City = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { StateOrProvince = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { PostalCode = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { CountryId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { AddressTypeId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { GoogleMapsUrl = "http://invalid" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { Latitude = "999" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { Longitude = "999" })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { Street = Long(201) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { ExternalNumber = Long(21) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { InternalNumber = Long(21) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { BetweenStreet1 = Long(201) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { BetweenStreet2 = Long(201) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { Neighbourhood = Long(201) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { City = Long(151) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { StateOrProvince = Long(151) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { PostalCode = Long(21) })).IsValid.Should().BeFalse();
        validator.Validate(new AddAddressCommand(request with { GoogleMapsUrl = Long(501) })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddCustomer_Validates_All_Fields()
    {
        var validator = new AddCustomerCommandValidator();
        var request = new AddCustomerRequest(1, "CUST", "a@b.com", "123456", 1);
        validator.Validate(new AddCustomerCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddCustomerCommand(request with { Code = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddCustomerCommand(request with { Email = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddCustomerCommand(request with { Phone = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddCustomerCommand(request with { StatusId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddCorporateCustomer_Validates_All_Fields()
    {
        var validator = new AddCorporateCustomerCommandValidator();
        var request = new AddCorporateCustomerRequest(1, "C001", "corp@b.com", "1111", 1, "Acme", "TAX123");
        validator.Validate(new AddCorporateCustomerCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddCorporateCustomerCommand(request with { LegalName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddCorporateCustomerCommand(request with { TaxIdentification = "" })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddIndividualCustomer_Validates_All_Fields()
    {
        var validator = new AddIndividualCustomerCommandValidator();
        var request = new AddIndividualCustomerRequest(1, "I001", "ind@b.com", "1111", 1, "Jane", "Doe",
            DateTime.UtcNow.Date, "1234567890", 1);
        validator.Validate(new AddIndividualCustomerCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddIndividualCustomerCommand(request with { FirstName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddIndividualCustomerCommand(request with { LastName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddIndividualCustomerCommand(request with { IdentificationTypeId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddIndividualCustomerCommand(request with { IdentificationNumber = Long(501) })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddCustomerAddress_Validates_All_Fields()
    {
        var validator = new AddCustomerAddressCommandValidator();
        var request = new AddCustomerAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        validator.Validate(new AddCustomerAddressCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddCustomerAddressCommand(request with { CustomerId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddCustomerAddressCommand(request with { AddressId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddBranchOffice_Validates_All_Fields()
    {
        var validator = new AddBranchOfficeCommandValidator();
        var request = new AddBranchOfficeRequest(1, "HQ", "1111", 2);
        validator.Validate(new AddBranchOfficeCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddBranchOfficeCommand(request with { Name = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddBranchOfficeCommand(request with { PhoneNumber = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddBranchOfficeCommand(request with { CorporateCustomerId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddBranchOfficeAddress_Validates_All_Fields()
    {
        var validator = new AddBranchOfficeAddressCommandValidator();
        var request = new AddBranchOfficeAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        validator.Validate(new AddBranchOfficeAddressCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddBranchOfficeAddressCommand(request with { BranchOfficeId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddBranchOfficeAddressCommand(request with { AddressId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddBankInformation_Validates_All_Fields()
    {
        var validator = new AddBankInformationCommandValidator();
        var request = new AddBankInformationRequest(1, "IBAN", "BIC", "Bank", 2);
        validator.Validate(new AddBankInformationCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddBankInformationCommand(request with { Iban = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddBankInformationCommand(request with { Bic = Long(501) })).IsValid.Should().BeFalse();
        validator.Validate(new AddBankInformationCommand(request with { BankName = Long(501) })).IsValid.Should().BeFalse();
        validator.Validate(new AddBankInformationCommand(request with { CustomerId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddTaxInformation_Validates_All_Fields()
    {
        var validator = new AddTaxInformationCommandValidator();
        var request = new AddTaxInformationRequest(1, "Tax Name", "TIN123", 2);
        validator.Validate(new AddTaxInformationCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddTaxInformationCommand(request with { TaxName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddTaxInformationCommand(request with { TaxIdentificationNumber = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddTaxInformationCommand(request with { CustomerId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddTaxInformationAddress_Validates_All_Fields()
    {
        var validator = new AddTaxInformationAddressCommandValidator();
        var request = new AddTaxInformationAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        validator.Validate(new AddTaxInformationAddressCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddTaxInformationAddressCommand(request with { TaxInformationId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddTaxInformationAddressCommand(request with { AddressId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddCreditAccount_Validates_All_Fields()
    {
        var validator = new AddCreditAccountCommandValidator();
        var request = new AddCreditAccountRequest(1, 1000m, 100m, 2);
        validator.Validate(new AddCreditAccountCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddCreditAccountCommand(request with { CustomerId = 0 })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddCreditTransaction_Validates_All_Fields()
    {
        var validator = new AddCreditTransactionCommandValidator();
        var request = new AddCreditTransactionRequest(1, DateTime.UtcNow.Date, 100m, "Consumo", "EXT", "Ok", 2);
        validator.Validate(new AddCreditTransactionCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddCreditTransactionCommand(request with { CreditAccountId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddCreditTransactionCommand(request with { Type = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddCreditTransactionCommand(request with { ExternalReference = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddCreditTransactionCommand(request with { Comments = "" })).IsValid.Should().BeFalse();
    }

    private static string Long(int length) => new('X', length);
}
