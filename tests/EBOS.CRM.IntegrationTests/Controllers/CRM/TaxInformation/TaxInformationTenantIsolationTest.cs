using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;
using CRMTaxInformation = EBOS.CRM.Domain.Entities.CRM.TaxInformation;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.TaxInformation;

public class TaxInformationTenantIsolationTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "TaxInformation");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var taxName1 = $"TaxName-{Guid.NewGuid():N}";
        var taxName2 = $"TaxName-{Guid.NewGuid():N}";
        var erasedTaxName = SeedTaxInformation(taxName1, taxName2, out var _);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/TaxInformation");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<TaxInformationResponse>();
        itemsTenant1.Should().Contain(i => i.TaxName == taxName1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.TaxName == taxName2);
        itemsTenant1.Should().NotContain(i => i.TaxName == erasedTaxName);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/TaxInformation");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<TaxInformationResponse>();
        itemsTenant2.Should().Contain(i => i.TaxName == taxName2);
        itemsTenant2.Should().NotContain(i => i.TaxName == taxName1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var taxName1 = $"TaxName-{Guid.NewGuid():N}";
        var taxName2 = $"TaxName-{Guid.NewGuid():N}";
        SeedTaxInformation(taxName1, taxName2, out var ids);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/TaxInformation/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private string SeedTaxInformation(string taxName1, string taxName2, out (long Tenant1Id, long Tenant2Id) ids)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var erasedTaxName = $"TaxName-Erased-{Guid.NewGuid():N}";

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

        var customerErased = new CRMCustomer
        {
            TenantId = 1,
            Code = $"C3-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "300",
            StatusId = statusId
        };

        db.Customers.AddRange(customer1, customer2, customerErased);
        db.SaveChanges();

        var tax1 = new CRMTaxInformation
        {
            TenantId = 1,
            CustomerId = customer1.Id,
            TaxName = taxName1,
            TaxIdentificationNumber = "TIN-1"
        };

        var tax2 = new CRMTaxInformation
        {
            TenantId = 2,
            CustomerId = customer2.Id,
            TaxName = taxName2,
            TaxIdentificationNumber = "TIN-2"
        };

        var taxErased = new CRMTaxInformation
        {
            TenantId = 1,
            CustomerId = customerErased.Id,
            TaxName = erasedTaxName,
            TaxIdentificationNumber = "TIN-3",
            Erased = true
        };

        db.TaxInformation.AddRange(tax1, tax2, taxErased);
        db.SaveChanges();

        ids = (tax1.Id, tax2.Id);
        return erasedTaxName;
    }

}

