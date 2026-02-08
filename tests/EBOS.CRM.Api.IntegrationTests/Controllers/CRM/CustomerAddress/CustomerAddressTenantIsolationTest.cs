using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMAddress = EBOS.CRM.Domain.Entities.CRM.Address;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;
using CRMCustomerAddress = EBOS.CRM.Domain.Entities.CRM.CustomerAddress;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.CustomerAddress;

public class CustomerAddressTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public CustomerAddressTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerAddress");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        SeedCustomerAddresses(out var data);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/CustomerAddress");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<CustomerAddressResponse>();
        itemsTenant1.Should().Contain(i => i.AddressId == data.Address1Id && i.Active);
        itemsTenant1.Should().NotContain(i => i.AddressId == data.Address2Id);
        itemsTenant1.Should().NotContain(i => i.AddressId == data.ErasedAddressId);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/CustomerAddress");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<CustomerAddressResponse>();
        itemsTenant2.Should().Contain(i => i.AddressId == data.Address2Id);
        itemsTenant2.Should().NotContain(i => i.AddressId == data.Address1Id);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        SeedCustomerAddresses(out var data);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/CustomerAddress/{data.CustomerAddress1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private void SeedCustomerAddresses(out (long CustomerAddress1Id, long CustomerAddress2Id, long Address1Id, long Address2Id, long ErasedAddressId) data)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var countryId = db.Countries.Select(c => c.Id).First();
        var addressTypeId = db.AddressTypes.Select(a => a.Id).First();

        var customer1 = new CRMCustomer
        {
            TenantId = 1,
            Code = $"C1-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "100",
            StatusId = statusId
        };

        var customer2 = new CRMCustomer
        {
            TenantId = 2,
            Code = $"C2-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "200",
            StatusId = statusId
        };

        db.Customers.AddRange(customer1, customer2);
        db.SaveChanges();

        var address1 = new CRMAddress
        {
            TenantId = 1,
            Street = "Street 1",
            ExternalNumber = "10",
            City = "City1",
            StateOrProvince = "State1",
            PostalCode = "11111",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

        var address2 = new CRMAddress
        {
            TenantId = 2,
            Street = "Street 2",
            ExternalNumber = "20",
            City = "City2",
            StateOrProvince = "State2",
            PostalCode = "22222",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

        var addressErased = new CRMAddress
        {
            TenantId = 1,
            Street = "Street 3",
            ExternalNumber = "30",
            City = "City3",
            StateOrProvince = "State3",
            PostalCode = "33333",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

        db.Addresses.AddRange(address1, address2, addressErased);
        db.SaveChanges();

        var link1 = new CRMCustomerAddress
        {
            TenantId = 1,
            CustomerId = customer1.Id,
            AddressId = address1.Id,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var link2 = new CRMCustomerAddress
        {
            TenantId = 2,
            CustomerId = customer2.Id,
            AddressId = address2.Id,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var linkErased = new CRMCustomerAddress
        {
            TenantId = 1,
            CustomerId = customer1.Id,
            AddressId = addressErased.Id,
            IsPrimary = false,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            Erased = true
        };

        db.Set<CRMCustomerAddress>().AddRange(link1, link2, linkErased);
        db.SaveChanges();

        data = (link1.Id, link2.Id, address1.Id, address2.Id, addressErased.Id);
    }

}
