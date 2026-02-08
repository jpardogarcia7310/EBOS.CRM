using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Address;

public class AddressTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public AddressTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "Address");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var streetTenant1 = $"Tenant1-{Guid.NewGuid():N}";
        var streetTenant2 = $"Tenant2-{Guid.NewGuid():N}";
        var data = SeedAddresses(streetTenant1, streetTenant2);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/Address");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<AddressResponse>();

        itemsTenant1.Should().Contain(i => i.Street == streetTenant1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.Street == streetTenant2);
        itemsTenant1.Should().NotContain(i => i.Street == data.ErasedStreet);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/Address");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<AddressResponse>();

        itemsTenant2.Should().Contain(i => i.Street == streetTenant2);
        itemsTenant2.Should().NotContain(i => i.Street == streetTenant1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var streetTenant1 = $"Tenant1-{Guid.NewGuid():N}";
        var streetTenant2 = $"Tenant2-{Guid.NewGuid():N}";
        var addressIds = SeedAddresses(streetTenant1, streetTenant2);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/Address/{addressIds.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private (long Tenant1Id, long Tenant2Id, string ErasedStreet) SeedAddresses(string streetTenant1, string streetTenant2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var erasedStreet = $"Erased-{Guid.NewGuid():N}";

        var address1 = new global::EBOS.CRM.Domain.Entities.CRM.Address
        {
            TenantId = 1,
            Street = streetTenant1,
            ExternalNumber = "10",
            City = "Quito",
            StateOrProvince = "Pichincha",
            PostalCode = "EC17001",
            CountryId = 1,
            AddressTypeId = 1
        };

        var address2 = new global::EBOS.CRM.Domain.Entities.CRM.Address
        {
            TenantId = 2,
            Street = streetTenant2,
            ExternalNumber = "20",
            City = "Quito",
            StateOrProvince = "Pichincha",
            PostalCode = "EC17001",
            CountryId = 1,
            AddressTypeId = 1
        };

        var erasedAddress = new global::EBOS.CRM.Domain.Entities.CRM.Address
        {
            TenantId = 1,
            Street = erasedStreet,
            ExternalNumber = "30",
            City = "Quito",
            StateOrProvince = "Pichincha",
            PostalCode = "EC17001",
            CountryId = 1,
            AddressTypeId = 1,
            Erased = true
        };

        db.Addresses.AddRange(address1, address2, erasedAddress);
        db.SaveChanges();

        return (address1.Id, address2.Id, erasedStreet);
    }

}

