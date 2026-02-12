using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Opportunity;

public class OpportunityTenantIsolationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var oppTenant1 = $"Tenant1-{Guid.NewGuid():N}";
        var oppTenant2 = $"Tenant2-{Guid.NewGuid():N}";
        var data = SeedOpportunities(oppTenant1, oppTenant2);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/Opportunity");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<OpportunityResponse>();

        itemsTenant1.Should().Contain(i => i.Name == oppTenant1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.Name == oppTenant2);
        itemsTenant1.Should().NotContain(i => i.Name == data.ErasedName);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/Opportunity");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<OpportunityResponse>();

        itemsTenant2.Should().Contain(i => i.Name == oppTenant2);
        itemsTenant2.Should().NotContain(i => i.Name == oppTenant1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var oppTenant1 = $"Tenant1-{Guid.NewGuid():N}";
        var oppTenant2 = $"Tenant2-{Guid.NewGuid():N}";
        var ids = SeedOpportunities(oppTenant1, oppTenant2);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/Opportunity/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private (long Tenant1Id, long Tenant2Id, string ErasedName) SeedOpportunities(string nameTenant1, string nameTenant2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var stageId = db.OpportunityStages.Select(s => s.Id).First();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var identificationTypeId = db.IdentificationTypes.Select(i => i.Id).First();
        var erasedName = $"Erased-{Guid.NewGuid():N}";

        var customer1 = CreateCustomer(db, 1, statusId, identificationTypeId);
        var customer2 = CreateCustomer(db, 2, statusId, identificationTypeId);

        var opp1 = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = nameTenant1,
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = customer1.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(5),
            Amount = 1000m,
            Probability = 0.5m,
            Source = "Web",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var opp2 = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 2,
            Name = nameTenant2,
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = customer2.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(5),
            Amount = 800m,
            Probability = 0.3m,
            Source = "Web",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var erased = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = erasedName,
            StageId = stageId,
            OwnerUserId = 1,
            CustomerId = customer1.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(5),
            Amount = 500m,
            Probability = 0.2m,
            Source = "Web",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            Erased = true
        };

        db.Opportunities.AddRange(opp1, opp2, erased);
        db.SaveChanges();

        return (opp1.Id, opp2.Id, erasedName);
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
