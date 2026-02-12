using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Quote;

public class QuoteTenantIsolationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var quoteTenant1 = $"QT1-{Guid.NewGuid():N}";
        var quoteTenant2 = $"QT2-{Guid.NewGuid():N}";
        var data = SeedQuotes(quoteTenant1, quoteTenant2);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/Quote");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<QuoteResponse>();

        itemsTenant1.Should().Contain(i => i.ReferenceNumber == quoteTenant1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.ReferenceNumber == quoteTenant2);
        itemsTenant1.Should().NotContain(i => i.ReferenceNumber == data.ErasedReference);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/Quote");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<QuoteResponse>();

        itemsTenant2.Should().Contain(i => i.ReferenceNumber == quoteTenant2);
        itemsTenant2.Should().NotContain(i => i.ReferenceNumber == quoteTenant1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var quoteTenant1 = $"QT1-{Guid.NewGuid():N}";
        var quoteTenant2 = $"QT2-{Guid.NewGuid():N}";
        var ids = SeedQuotes(quoteTenant1, quoteTenant2);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/Quote/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private (long Tenant1Id, long Tenant2Id, string ErasedReference) SeedQuotes(
        string referenceTenant1,
        string referenceTenant2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var stageId = db.OpportunityStages.Select(s => s.Id).First();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var identificationTypeId = db.IdentificationTypes.Select(i => i.Id).First();
        var erasedReference = $"ER-{Guid.NewGuid():N}";

        var customer1 = CreateCustomer(db, 1, statusId, identificationTypeId);
        var customer2 = CreateCustomer(db, 2, statusId, identificationTypeId);

        var opp1 = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = $"Opp-{Guid.NewGuid():N}",
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
            Name = $"Opp-{Guid.NewGuid():N}",
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

        var quote1 = new Domain.Entities.CRM.Quote
        {
            TenantId = 1,
            Opportunity = opp1,
            Status = "Draft",
            ReferenceNumber = referenceTenant1,
            SubtotalAmount = 1000m,
            DiscountAmount = 100m,
            TotalAmount = 900m,
            ValidUntil = DateTime.UtcNow.AddDays(15),
            Notes = "Quote 1",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var quote2 = new Domain.Entities.CRM.Quote
        {
            TenantId = 2,
            Opportunity = opp2,
            Status = "Draft",
            ReferenceNumber = referenceTenant2,
            SubtotalAmount = 800m,
            DiscountAmount = 0m,
            TotalAmount = 800m,
            ValidUntil = DateTime.UtcNow.AddDays(15),
            Notes = "Quote 2",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var erased = new Domain.Entities.CRM.Quote
        {
            TenantId = 1,
            Opportunity = opp1,
            Status = "Draft",
            ReferenceNumber = erasedReference,
            SubtotalAmount = 500m,
            DiscountAmount = 0m,
            TotalAmount = 500m,
            ValidUntil = DateTime.UtcNow.AddDays(15),
            Notes = "Erased",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            Erased = true
        };

        db.Customers.AddRange(customer1, customer2);
        db.Opportunities.AddRange(opp1, opp2);
        db.Quotes.AddRange(quote1, quote2, erased);
        db.SaveChanges();

        return (quote1.Id, quote2.Id, erasedReference);
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
