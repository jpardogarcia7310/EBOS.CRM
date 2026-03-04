using System.Net;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Opportunity;

public class OpportunityConcurrencyTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;
    private readonly long _opportunityId;

    public OpportunityConcurrencyTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");
        _opportunityId = EnsureOpportunity(factory);
    }

    [Fact]
    public async Task Stress_GetAll_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync($"/api/v{_version}/Opportunity"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Stress_GetById_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync($"/api/v{_version}/Opportunity/{_opportunityId}"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    private static long EnsureOpportunity(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var existing = db.Opportunities.FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
        }

        var stageId = db.OpportunityStages.Select(s => s.Id).First();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var identificationTypeId = db.IdentificationTypes.Select(i => i.Id).First();

        var customer = new Domain.Entities.CRM.IndividualCustomer
        {
            TenantId = 1,
            Code = $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email = $"customer{Guid.NewGuid():N}@example.com",
            Phone = "1234567890",
            StatusId = statusId,
            FirstName = "John",
            LastName = "Doe",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            IdentificationNumber = "1234567890",
            IdentificationTypeId = identificationTypeId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        var opportunity = new Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = $"Concurrent-{Guid.NewGuid():N}",
            StageId = stageId,
            OwnerUserId = 1,
            Customer = customer,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(5),
            Amount = 1000m,
            Probability = 0.5m,
            Source = "Web",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.Customers.Add(customer);
        db.Opportunities.Add(opportunity);
        db.SaveChanges();
        return opportunity.Id;
    }
}
