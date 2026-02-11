using System.Net;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadTenantIsolationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var leadTenant1 = $"Tenant1-{Guid.NewGuid():N}";
        var leadTenant2 = $"Tenant2-{Guid.NewGuid():N}";
        var data = SeedLeads(leadTenant1, leadTenant2);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/Lead");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<LeadResponse>();

        itemsTenant1.Should().Contain(i => i.CompanyName == leadTenant1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.CompanyName == leadTenant2);
        itemsTenant1.Should().NotContain(i => i.CompanyName == data.ErasedCompany);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/Lead");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<LeadResponse>();

        itemsTenant2.Should().Contain(i => i.CompanyName == leadTenant2);
        itemsTenant2.Should().NotContain(i => i.CompanyName == leadTenant1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var leadTenant1 = $"Tenant1-{Guid.NewGuid():N}";
        var leadTenant2 = $"Tenant2-{Guid.NewGuid():N}";
        var leadIds = SeedLeads(leadTenant1, leadTenant2);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/Lead/{leadIds.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private (long Tenant1Id, long Tenant2Id, string ErasedCompany) SeedLeads(string companyTenant1, string companyTenant2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var erasedCompany = $"Erased-{Guid.NewGuid():N}";

        var lead1 = new Domain.Entities.CRM.Lead
        {
            TenantId = 1,
            Source = "Web",
            Status = "New",
            OwnerUserId = 1,
            CompanyName = companyTenant1,
            ContactName = "Alice",
            Email = $"lead{Guid.NewGuid():N}@example.com",
            Phone = "1234567890",
            EstimatedValue = 500m,
            Notes = "Lead 1",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var lead2 = new Domain.Entities.CRM.Lead
        {
            TenantId = 2,
            Source = "Web",
            Status = "New",
            OwnerUserId = 1,
            CompanyName = companyTenant2,
            ContactName = "Bob",
            Email = $"lead{Guid.NewGuid():N}@example.com",
            Phone = "1234567890",
            EstimatedValue = 800m,
            Notes = "Lead 2",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var erasedLead = new Domain.Entities.CRM.Lead
        {
            TenantId = 1,
            Source = "Referral",
            Status = "Closed",
            OwnerUserId = 1,
            CompanyName = erasedCompany,
            ContactName = "Eve",
            Email = $"lead{Guid.NewGuid():N}@example.com",
            Phone = "1234567890",
            EstimatedValue = 100m,
            Notes = "Erased lead",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            Erased = true
        };

        db.Leads.AddRange(lead1, lead2, erasedLead);
        db.SaveChanges();

        return (lead1.Id, lead2.Id, erasedCompany);
    }
}
