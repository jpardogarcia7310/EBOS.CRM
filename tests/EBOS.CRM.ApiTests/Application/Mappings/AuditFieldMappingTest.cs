using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Domain.Entities.CRM;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class AuditFieldMappingTest(MapperFixture fixture) : IClassFixture<MapperFixture>
{
    private readonly IMapper _mapper = fixture.Mapper;

    [Fact]
    public void UpdateCustomerRequest_DoesNotOverwrite_AuditFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAt = DateTime.UtcNow.AddDays(-1);
        var entity = new Customer
        {
            CreatedAt = createdAt,
            CreatedBy = 10,
            UpdatedAt = updatedAt,
            UpdatedBy = 20
        };

        var request = new UpdateCustomerRequest(
            Id: 1,
            TenantId: 3,
            Code: "C-01",
            Email: "test@site.com",
            Phone: "123",
            StatusId: 1);

        _mapper.Map(request, entity);

        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(10, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(20, entity.UpdatedBy);
    }

    [Fact]
    public void UpdateAddressRequest_DoesNotOverwrite_AuditFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-5);
        var updatedAt = DateTime.UtcNow.AddDays(-2);
        var entity = new Address
        {
            CreatedAt = createdAt,
            CreatedBy = 11,
            UpdatedAt = updatedAt,
            UpdatedBy = 21
        };

        var request = new UpdateAddressRequest(
            TenantId: 2,
            Street: "Main",
            ExternalNumber: "1",
            InternalNumber: null,
            BetweenStreet1: null,
            BetweenStreet2: null,
            Neighbourhood: null,
            City: "Quito",
            StateOrProvince: "Pichincha",
            PostalCode: "EC17001",
            GoogleMapsUrl: null,
            Latitude: null,
            Longitude: null,
            CountryId: 1,
            AddressTypeId: 1);

        _mapper.Map(request, entity);

        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(11, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(21, entity.UpdatedBy);
    }

    [Fact]
    public void UpdateBankInformationRequest_DoesNotOverwrite_AuditFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-8);
        var updatedAt = DateTime.UtcNow.AddDays(-3);
        var entity = new BankInformation
        {
            CreatedAt = createdAt,
            CreatedBy = 12,
            UpdatedAt = updatedAt,
            UpdatedBy = 22
        };

        var request = new EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation.UpdateBankInformationRequest(
            TenantId: 2,
            Iban: "IBAN",
            Bic: "BIC",
            BankName: "Bank",
            CustomerId: 1);

        _mapper.Map(request, entity);

        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(12, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(22, entity.UpdatedBy);
    }

    [Fact]
    public void UpdateBranchOfficeRequest_DoesNotOverwrite_AuditFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-6);
        var updatedAt = DateTime.UtcNow.AddDays(-4);
        var entity = new BranchOffice
        {
            CreatedAt = createdAt,
            CreatedBy = 13,
            UpdatedAt = updatedAt,
            UpdatedBy = 23
        };

        var request = new EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice.UpdateBranchOfficeRequest(
            Id: 1,
            TenantId: 2,
            Name: "Branch",
            PhoneNumber: "123",
            CorporateCustomerId: 1);

        _mapper.Map(request, entity);

        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(13, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(23, entity.UpdatedBy);
    }

    [Fact]
    public void UpdateCreditAccountRequest_DoesNotOverwrite_AuditFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-9);
        var updatedAt = DateTime.UtcNow.AddDays(-2);
        var entity = new CreditAccount
        {
            CreatedAt = createdAt,
            CreatedBy = 14,
            UpdatedAt = updatedAt,
            UpdatedBy = 24
        };

        var request = new EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount.UpdateCreditAccountRequest(
            Id: 1,
            TenantId: 2,
            MaxAmount: 100,
            UsedAmount: 10,
            CustomerId: 1);

        _mapper.Map(request, entity);

        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(14, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(24, entity.UpdatedBy);
    }
}
