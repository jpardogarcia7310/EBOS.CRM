using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class AuditFieldResponseMappingPerEntityTest(MapperFixture fixture) : IClassFixture<MapperFixture>
{
    private static readonly string[] AuditFields = ["CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy"];
    private readonly IMapper _mapper = fixture.Mapper;

    [Fact]
    public void Address_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new Address
        {
            Id = 10,
            TenantId = 1,
            Street = "Street",
            ExternalNumber = "10",
            City = "City",
            StateOrProvince = "State",
            PostalCode = "00000",
            CountryId = 1,
            AddressTypeId = 1
        };

        var response = _mapper.Map<AddressResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void BankInformation_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new BankInformation
        {
            Id = 11,
            TenantId = 1,
            Iban = "IBAN",
            Bic = "BIC",
            BankName = "Bank",
            CustomerId = 100
        };

        var response = _mapper.Map<BankInformationResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void BranchOffice_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new BranchOffice
        {
            Id = 12,
            TenantId = 1,
            Name = "Branch",
            PhoneNumber = "111",
            CorporateCustomerId = 200
        };

        var response = _mapper.Map<BranchOfficeResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void BranchOfficeAddress_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new BranchOfficeAddress
        {
            Id = 13,
            TenantId = 1,
            BranchOfficeId = 12,
            AddressId = 10,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var response = _mapper.Map<BranchOfficeAddressResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void CorporateCustomer_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new CorporateCustomer
        {
            Id = 14,
            TenantId = 1,
            Code = "C1",
            Email = "a@site.com",
            Phone = "100",
            StatusId = 1,
            LegalName = "Legal",
            TaxIdentification = "TAX"
        };

        var response = _mapper.Map<CorporateCustomerResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void CreditAccount_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new CreditAccount
        {
            Id = 15,
            TenantId = 1,
            MaxAmount = 1000m,
            UsedAmount = 200m,
            CustomerId = 100
        };

        var response = _mapper.Map<CreditAccountResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void CreditTransaction_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new CreditTransaction
        {
            Id = 16,
            TenantId = 1,
            Date = DateTime.UtcNow,
            Amount = 50m,
            Type = "Consumption",
            ExternalReference = "EXT",
            Comments = "Note",
            CreditAccountId = 15
        };

        var response = _mapper.Map<CreditTransactionResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void Customer_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new Customer
        {
            Id = 17,
            TenantId = 1,
            Code = "CUST",
            Email = "cust@site.com",
            Phone = "123",
            StatusId = 1
        };

        var response = _mapper.Map<CustomerResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void CustomerAddress_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new CustomerAddress
        {
            Id = 18,
            TenantId = 1,
            CustomerId = 17,
            AddressId = 10,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var response = _mapper.Map<CustomerAddressResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void IndividualCustomer_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new IndividualCustomer
        {
            Id = 19,
            TenantId = 1,
            Code = "IC1",
            Email = "ic@site.com",
            Phone = "555",
            StatusId = 1,
            FirstName = "First",
            LastName = "Last",
            BirthDate = new DateTime(1990, 1, 1),
            IdentificationNumber = "ID-1",
            IdentificationTypeId = 1
        };

        var response = _mapper.Map<IndividualCustomerResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void TaxInformation_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new TaxInformation
        {
            Id = 20,
            TenantId = 1,
            TaxName = "Tax",
            TaxIdentificationNumber = "TIN",
            CustomerId = 17
        };

        var response = _mapper.Map<TaxInformationResponse>(entity);
        AssertNoAuditFields(response);
    }

    [Fact]
    public void TaxInformationAddress_Response_Does_Not_Expose_AuditFields()
    {
        var entity = new TaxInformationAddress
        {
            Id = 21,
            TenantId = 1,
            TaxInformationId = 20,
            AddressId = 10,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var response = _mapper.Map<TaxInformationAddressResponse>(entity);
        AssertNoAuditFields(response);
    }

    private static void AssertNoAuditFields<TResponse>(TResponse response)
    {
        response.Should().NotBeNull();
        var properties = typeof(TResponse).GetProperties().Select(p => p.Name).ToList();
        properties.Should().NotContain(AuditFields, $"response {typeof(TResponse).Name} must not expose audit fields");
    }
}
