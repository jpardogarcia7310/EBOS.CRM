using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCorporateCustomer = EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.CorporateCustomer;

public class CorporateCustomerTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public CorporateCustomerTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var legal1 = $"Legal-{Guid.NewGuid():N}";
        var legal2 = $"Legal-{Guid.NewGuid():N}";
        var erasedLegalName = SeedCorporateCustomers(legal1, legal2, out var _);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/CorporateCustomer");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<CorporateCustomerResponse>();
        itemsTenant1.Should().Contain(i => i.LegalName == legal1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.LegalName == legal2);
        itemsTenant1.Should().NotContain(i => i.LegalName == erasedLegalName);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/CorporateCustomer");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<CorporateCustomerResponse>();
        itemsTenant2.Should().Contain(i => i.LegalName == legal2);
        itemsTenant2.Should().NotContain(i => i.LegalName == legal1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var legal1 = $"Legal-{Guid.NewGuid():N}";
        var legal2 = $"Legal-{Guid.NewGuid():N}";
        SeedCorporateCustomers(legal1, legal2, out var ids);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/CorporateCustomer/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private string SeedCorporateCustomers(string legal1, string legal2, out (long Tenant1Id, long Tenant2Id) ids)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var erasedLegalName = $"Legal-Erased-{Guid.NewGuid():N}";

        var customer1 = new CRMCorporateCustomer
        {
            TenantId = 1,
            Code = $"C1-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "100",
            StatusId = statusId,
            LegalName = legal1,
            TaxIdentification = "TAX-1"
        };

        var customer2 = new CRMCorporateCustomer
        {
            TenantId = 2,
            Code = $"C2-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "200",
            StatusId = statusId,
            LegalName = legal2,
            TaxIdentification = "TAX-2"
        };

        var customerErased = new CRMCorporateCustomer
        {
            TenantId = 1,
            Code = $"C3-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "300",
            StatusId = statusId,
            LegalName = erasedLegalName,
            TaxIdentification = "TAX-3",
            Erased = true
        };

        db.CorporateCustomers.AddRange(customer1, customer2, customerErased);
        db.SaveChanges();

        ids = (customer1.Id, customer2.Id);
        return erasedLegalName;
    }

}
