using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = global::EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Customer;

public class CustomerTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public CustomerTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var codeTenant1 = $"C1-{Guid.NewGuid():N}";
        var codeTenant2 = $"C2-{Guid.NewGuid():N}";
        var erasedCode = SeedCustomers(codeTenant1, codeTenant2, out var _);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/Customer");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<CustomerResponse>();

        itemsTenant1.Should().Contain(i => i.Code == codeTenant1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.Code == codeTenant2);
        itemsTenant1.Should().NotContain(i => i.Code == erasedCode);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/Customer");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<CustomerResponse>();

        itemsTenant2.Should().Contain(i => i.Code == codeTenant2);
        itemsTenant2.Should().NotContain(i => i.Code == codeTenant1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var codeTenant1 = $"C1-{Guid.NewGuid():N}";
        var codeTenant2 = $"C2-{Guid.NewGuid():N}";
        SeedCustomers(codeTenant1, codeTenant2, out var ids);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/Customer/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_Uses_Header_When_Subdomain_Present()
    {
        var codeTenant1 = $"C1-{Guid.NewGuid():N}";
        var codeTenant2 = $"C2-{Guid.NewGuid():N}";
        SeedCustomers(codeTenant1, codeTenant2, out _);

        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v{_version}/Customer");
        request.Headers.Add("X-Tenant-Id", "1");
        request.Headers.Host = "tenant2.api.domain";

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<CustomerResponse>();

        items.Should().Contain(i => i.Code == codeTenant1);
        items.Should().NotContain(i => i.Code == codeTenant2);
    }

    [Fact]
    public async Task GetAll_Uses_Subdomain_When_Header_Missing()
    {
        var codeTenant1 = $"C1-{Guid.NewGuid():N}";
        var codeTenant2 = $"C2-{Guid.NewGuid():N}";
        SeedCustomers(codeTenant1, codeTenant2, out _);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Remove("X-Tenant-Id");
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v{_version}/Customer");
        request.Headers.Host = "tenant2.api.domain";

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadItemsAsync<CustomerResponse>();

        items.Should().Contain(i => i.Code == codeTenant2);
        items.Should().NotContain(i => i.Code == codeTenant1);
    }

    [Fact]
    public async Task Add_Returns_400_When_TenantId_Missing_In_Request()
    {
        var client = HttpClientFactory.CreateClientWithTenant(_factory, 5);
        var statusId = await LookupHelper.GetStatusIdAsync(client, ApiVersionHelper.GetLatestVersion(_factory, "Status"));

        var addRequest = new global::EBOS.CRM.Application.Contracts.Requests.CRM.Customer.AddCustomerRequest(
            TenantId: 0,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"auto{Guid.NewGuid():N}@example.com",
            Phone: "+34 600 555 555",
            StatusId: statusId);

        var addResponse = await client.PostAsJsonAsync($"/api/v{_version}/Customer", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private string SeedCustomers(string codeTenant1, string codeTenant2, out (long Tenant1Id, long Tenant2Id) ids)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var erasedCode = $"CE-{Guid.NewGuid():N}";

        var customer1 = new CRMCustomer
        {
            TenantId = 1,
            Code = codeTenant1,
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "100",
            StatusId = statusId
        };

        var customer2 = new CRMCustomer
        {
            TenantId = 2,
            Code = codeTenant2,
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "200",
            StatusId = statusId
        };

        var erasedCustomer = new CRMCustomer
        {
            TenantId = 1,
            Code = erasedCode,
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "300",
            StatusId = statusId,
            Erased = true
        };

        db.Customers.AddRange(customer1, customer2, erasedCustomer);
        db.SaveChanges();

        ids = (customer1.Id, customer2.Id);
        return erasedCode;
    }

}


