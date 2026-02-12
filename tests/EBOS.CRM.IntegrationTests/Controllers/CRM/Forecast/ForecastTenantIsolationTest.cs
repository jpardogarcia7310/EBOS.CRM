using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Forecast;

public class ForecastTenantIsolationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Forecast");

    [Fact]
    public async Task GetSummary_Filters_By_Tenant_Header()
    {
        SeedOpportunities();

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/forecast");
        response1.EnsureSuccessStatusCode();
        var summary1 = await response1.Content.ReadFromJsonAsync<ForecastSummaryResponse>();
        summary1.Should().NotBeNull();
        summary1.TotalAmount.Should().Be(1000m);
        summary1.WeightedAmount.Should().Be(500m);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/forecast");
        response2.EnsureSuccessStatusCode();
        var summary2 = await response2.Content.ReadFromJsonAsync<ForecastSummaryResponse>();
        summary2.Should().NotBeNull();
        summary2.TotalAmount.Should().Be(800m);
        summary2.WeightedAmount.Should().Be(400m);
    }

    private void SeedOpportunities()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        if (db.Opportunities.Any())
        {
            return;
        }

        var stageId = db.OpportunityStages.Select(s => s.Id).First();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var identificationTypeId = db.IdentificationTypes.Select(i => i.Id).First();

        var customer1 = CreateCustomer(db, 1, statusId, identificationTypeId);
        var customer2 = CreateCustomer(db, 2, statusId, identificationTypeId);

        var opp1 = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = "Forecast Tenant1",
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = customer1.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(7),
            Amount = 1000m,
            Probability = 0.5m,
            Source = "Web",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var opp2 = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 2,
            Name = "Forecast Tenant2",
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = customer2.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(7),
            Amount = 800m,
            Probability = 0.5m,
            Source = "Web",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.Customers.AddRange(customer1, customer2);
        db.Opportunities.AddRange(opp1, opp2);
        db.SaveChanges();
    }

    private static Domain.Entities.CRM.IndividualCustomer CreateCustomer(
        CrmDbContext db,
        long tenantId,
        long statusId,
        long identificationTypeId)
    {
        var customer = new Domain.Entities.CRM.IndividualCustomer
        {
            TenantId = tenantId,
            Code = $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email = $"customer{Guid.NewGuid():N}@example.com",
            Phone = "1234567890",
            StatusId = statusId,
            FirstName = "Tenant",
            LastName = tenantId.ToString(),
            BirthDate = DateTime.UtcNow.AddYears(-25),
            IdentificationNumber = "1234567890",
            IdentificationTypeId = identificationTypeId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.Customers.Add(customer);
        return customer;
    }
}
