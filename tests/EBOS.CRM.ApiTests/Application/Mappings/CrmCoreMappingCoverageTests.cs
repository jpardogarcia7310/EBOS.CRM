using EBOS.CRM.ApiTests.Fixtures;
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
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class CrmCoreMappingCoverageTests(MapperFixture fixture) : IClassFixture<MapperFixture>
{
    private readonly IMapper _mapper = fixture.Mapper;

    [Fact]
    public void Address_Mapping_Covers_All_Fields()
    {
        var request = new AddAddressRequest(1, "Main", "1", "A", "B", "C", "N", "City",
            "State", "12345", "https://maps.test", "10.5", "-20.5", 1, 2);
        var entity = _mapper.Map<Address>(request);
        entity.TenantId.Should().Be(1);
        entity.Street.Should().Be("Main");
        entity.ExternalNumber.Should().Be("1");
        entity.InternalNumber.Should().Be("A");
        entity.BetweenStreet1.Should().Be("B");
        entity.BetweenStreet2.Should().Be("C");
        entity.City.Should().Be("City");
        entity.StateOrProvince.Should().Be("State");
        entity.PostalCode.Should().Be("12345");
        entity.GoogleMapsUrl.Should().Be("https://maps.test");
        entity.Latitude.Should().Be(10.5m);
        entity.Longitude.Should().Be(-20.5m);
        entity.CountryId.Should().Be(1);
        entity.AddressTypeId.Should().Be(2);

        var response = _mapper.Map<AddressResponse>(entity);
        response.TenantId.Should().Be(1);
        response.Latitude.Should().Be("10.5");
        response.Longitude.Should().Be("-20.5");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void Customer_Mapping_Covers_All_Fields()
    {
        var request = new AddCustomerRequest(1, "CUST", "a@b.com", "123456", 2);
        var entity = _mapper.Map<Customer>(request);
        entity.TenantId.Should().Be(1);
        entity.Code.Should().Be("CUST");
        entity.Email.Should().Be("a@b.com");
        entity.Phone.Should().Be("123456");
        entity.StatusId.Should().Be(2);

        var response = _mapper.Map<CustomerResponse>(entity);
        response.TenantId.Should().Be(1);
        response.Code.Should().Be("CUST");
        response.StatusId.Should().Be(2);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void CorporateCustomer_Mapping_Covers_All_Fields()
    {
        var request = new AddCorporateCustomerRequest(1, "C001", "corp@b.com", "1111", 1, "Acme", "TAX123");
        var entity = _mapper.Map<CorporateCustomer>(request);
        entity.TenantId.Should().Be(1);
        entity.Code.Should().Be("C001");
        entity.Email.Should().Be("corp@b.com");
        entity.Phone.Should().Be("1111");
        entity.StatusId.Should().Be(1);
        entity.LegalName.Should().Be("Acme");
        entity.TaxIdentification.Should().Be("TAX123");

        var response = _mapper.Map<CorporateCustomerResponse>(entity);
        response.TenantId.Should().Be(1);
        response.LegalName.Should().Be("Acme");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void IndividualCustomer_Mapping_Covers_All_Fields()
    {
        var request = new AddIndividualCustomerRequest(1, "I001", "ind@b.com", "1111", 1,
            "Jane", "Doe", DateTime.UtcNow.Date, "1234567890", 1);
        var entity = _mapper.Map<IndividualCustomer>(request);
        entity.TenantId.Should().Be(1);
        entity.Code.Should().Be("I001");
        entity.Email.Should().Be("ind@b.com");
        entity.Phone.Should().Be("1111");
        entity.StatusId.Should().Be(1);
        entity.FirstName.Should().Be("Jane");
        entity.LastName.Should().Be("Doe");
        entity.IdentificationNumber.Should().Be("1234567890");
        entity.IdentificationTypeId.Should().Be(1);

        var response = _mapper.Map<IndividualCustomerResponse>(entity);
        response.TenantId.Should().Be(1);
        response.FirstName.Should().Be("Jane");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void CustomerAddress_Mapping_Covers_All_Fields()
    {
        var request = new AddCustomerAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        var entity = _mapper.Map<CustomerAddress>(request);
        entity.TenantId.Should().Be(1);
        entity.CustomerId.Should().Be(2);
        entity.AddressId.Should().Be(3);
        entity.IsPrimary.Should().BeTrue();
        entity.IsCurrent.Should().BeTrue();

        var response = _mapper.Map<CustomerAddressResponse>(entity);
        response.TenantId.Should().Be(1);
        response.CustomerId.Should().Be(2);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void BranchOffice_Mapping_Covers_All_Fields()
    {
        var request = new AddBranchOfficeRequest(1, "HQ", "1111", 10);
        var entity = _mapper.Map<BranchOffice>(request);
        entity.TenantId.Should().Be(1);
        entity.Name.Should().Be("HQ");
        entity.PhoneNumber.Should().Be("1111");
        entity.CorporateCustomerId.Should().Be(10);

        var response = _mapper.Map<BranchOfficeResponse>(entity);
        response.TenantId.Should().Be(1);
        response.Name.Should().Be("HQ");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void BranchOfficeAddress_Mapping_Covers_All_Fields()
    {
        var request = new AddBranchOfficeAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        var entity = _mapper.Map<BranchOfficeAddress>(request);
        entity.TenantId.Should().Be(1);
        entity.BranchOfficeId.Should().Be(2);
        entity.AddressId.Should().Be(3);
        entity.IsPrimary.Should().BeTrue();
        entity.IsCurrent.Should().BeTrue();

        var response = _mapper.Map<BranchOfficeAddressResponse>(entity);
        response.TenantId.Should().Be(1);
        response.BranchOfficeId.Should().Be(2);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void BankInformation_Mapping_Covers_All_Fields()
    {
        var request = new AddBankInformationRequest(1, "IBAN", "BIC", "Bank", 5);
        var entity = _mapper.Map<BankInformation>(request);
        entity.TenantId.Should().Be(1);
        entity.Iban.Should().Be("IBAN");
        entity.Bic.Should().Be("BIC");
        entity.BankName.Should().Be("Bank");
        entity.CustomerId.Should().Be(5);

        var response = _mapper.Map<BankInformationResponse>(entity);
        response.TenantId.Should().Be(1);
        response.CustomerId.Should().Be(5);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void TaxInformation_Mapping_Covers_All_Fields()
    {
        var request = new AddTaxInformationRequest(1, "Tax Name", "TIN123", 3);
        var entity = _mapper.Map<TaxInformation>(request);
        entity.TenantId.Should().Be(1);
        entity.TaxName.Should().Be("Tax Name");
        entity.TaxIdentificationNumber.Should().Be("TIN123");
        entity.CustomerId.Should().Be(3);

        var response = _mapper.Map<TaxInformationResponse>(entity);
        response.TenantId.Should().Be(1);
        response.CustomerId.Should().Be(3);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void TaxInformationAddress_Mapping_Covers_All_Fields()
    {
        var request = new AddTaxInformationAddressRequest(1, 2, 3, true, DateTime.UtcNow.Date, null, true);
        var entity = _mapper.Map<TaxInformationAddress>(request);
        entity.TenantId.Should().Be(1);
        entity.TaxInformationId.Should().Be(2);
        entity.AddressId.Should().Be(3);
        entity.IsPrimary.Should().BeTrue();
        entity.IsCurrent.Should().BeTrue();

        var response = _mapper.Map<TaxInformationAddressResponse>(entity);
        response.TenantId.Should().Be(1);
        response.TaxInformationId.Should().Be(2);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void CreditAccount_Mapping_Covers_All_Fields()
    {
        var request = new AddCreditAccountRequest(1, 1000m, 100m, 5);
        var entity = _mapper.Map<CreditAccount>(request);
        entity.TenantId.Should().Be(1);
        entity.CustomerId.Should().Be(5);
        entity.MaxAmount.Should().Be(1000m);
        entity.UsedAmount.Should().Be(100m);

        var response = _mapper.Map<CreditAccountResponse>(entity);
        response.TenantId.Should().Be(1);
        response.CustomerId.Should().Be(5);
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void CreditTransaction_Mapping_Covers_All_Fields()
    {
        var request = new AddCreditTransactionRequest(1, DateTime.UtcNow.Date, 100m, "Consumo", "EXT", "Ok", 2);
        var entity = _mapper.Map<CreditTransaction>(request);
        entity.TenantId.Should().Be(1);
        entity.CreditAccountId.Should().Be(2);
        entity.Amount.Should().Be(100m);
        entity.Type.Should().Be("Consumo");
        entity.ExternalReference.Should().Be("EXT");
        entity.Comments.Should().Be("Ok");

        var response = _mapper.Map<CreditTransactionResponse>(entity);
        response.TenantId.Should().Be(1);
        response.CreditAccountId.Should().Be(2);
        response.Active.Should().BeTrue();
    }
}
