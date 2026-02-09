using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class ActiveMappingTest(MapperFixture fixture) : IClassFixture<MapperFixture>
{
    private readonly IMapper _mapper = fixture.Mapper;

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Address_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new Address
        {
            Id = 1,
            TenantId = 1,
            Street = "Street",
            ExternalNumber = "10",
            City = "City",
            StateOrProvince = "State",
            PostalCode = "00000",
            CountryId = 1,
            AddressTypeId = 1,
            Erased = erased
        };

        var response = _mapper.Map<AddressResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void BankInformation_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new BankInformation
        {
            Id = 2,
            TenantId = 1,
            Iban = "IBAN",
            CustomerId = 10,
            Erased = erased
        };

        var response = _mapper.Map<BankInformationResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void BranchOffice_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new BranchOffice
        {
            Id = 3,
            TenantId = 1,
            Name = "Branch",
            PhoneNumber = "111",
            CorporateCustomerId = 20,
            Erased = erased
        };

        var response = _mapper.Map<BranchOfficeResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void BranchOfficeAddress_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new BranchOfficeAddress
        {
            Id = 4,
            TenantId = 1,
            BranchOfficeId = 3,
            AddressId = 1,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            Erased = erased
        };

        var response = _mapper.Map<BranchOfficeAddressResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CorporateCustomer_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new CorporateCustomer
        {
            Id = 5,
            TenantId = 1,
            Code = "C1",
            Email = "a@site.com",
            Phone = "100",
            StatusId = 1,
            LegalName = "Legal",
            TaxIdentification = "TAX",
            Erased = erased
        };

        var response = _mapper.Map<CorporateCustomerResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CreditAccount_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new CreditAccount
        {
            Id = 6,
            TenantId = 1,
            MaxAmount = 1000m,
            UsedAmount = 100m,
            CustomerId = 10,
            Erased = erased
        };

        var response = _mapper.Map<CreditAccountResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CreditTransaction_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new CreditTransaction
        {
            Id = 7,
            TenantId = 1,
            Date = DateTime.UtcNow,
            Amount = 10m,
            Type = "Consumption",
            ExternalReference = "EXT",
            Comments = "Note",
            CreditAccountId = 6,
            Erased = erased
        };

        var response = _mapper.Map<CreditTransactionResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Customer_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new Customer
        {
            Id = 8,
            TenantId = 1,
            Code = "CUST",
            Email = "cust@site.com",
            Phone = "123",
            StatusId = 1,
            Erased = erased
        };

        var response = _mapper.Map<CustomerResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CustomerAddress_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new CustomerAddress
        {
            Id = 9,
            TenantId = 1,
            CustomerId = 8,
            AddressId = 1,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            Erased = erased
        };

        var response = _mapper.Map<CustomerAddressResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void IndividualCustomer_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new IndividualCustomer
        {
            Id = 10,
            TenantId = 1,
            Code = "IC1",
            Email = "ic@site.com",
            Phone = "555",
            StatusId = 1,
            FirstName = "First",
            LastName = "Last",
            BirthDate = new DateTime(1990, 1, 1),
            IdentificationNumber = "ID-1",
            IdentificationTypeId = 1,
            Erased = erased
        };

        var response = _mapper.Map<IndividualCustomerResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void TaxInformation_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new TaxInformation
        {
            Id = 11,
            TenantId = 1,
            TaxName = "Tax",
            TaxIdentificationNumber = "TIN",
            CustomerId = 8,
            Erased = erased
        };

        var response = _mapper.Map<TaxInformationResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void TaxInformationAddress_Maps_Active_From_Erased(bool erased, bool expectedActive)
    {
        var entity = new TaxInformationAddress
        {
            Id = 12,
            TenantId = 1,
            TaxInformationId = 11,
            AddressId = 1,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            Erased = erased
        };

        var response = _mapper.Map<TaxInformationAddressResponse>(entity);

        response.Active.Should().Be(expectedActive);
    }
}
