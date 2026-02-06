using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.Customer;

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
