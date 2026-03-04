using System.Net;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadConcurrencyTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;
    private readonly long _leadId;

    public LeadConcurrencyTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");
        _leadId = EnsureLead(factory);
    }

    [Fact]
    public async Task Stress_GetAll_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync($"/api/v{_version}/Lead"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task Stress_GetById_ConcurrentRequests_ReturnsConsistentResults()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => _client.GetAsync($"/api/v{_version}/Lead/{_leadId}"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    private static long EnsureLead(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var existing = db.Leads.FirstOrDefault();
        if (existing != null)
        {
            return existing.Id;
        }

        var lead = new Domain.Entities.CRM.Lead
        {
            TenantId = 1,
            Source = "Web",
            Status = "New",
            OwnerUserId = 1,
            CompanyName = $"Concurrent-{Guid.NewGuid():N}",
            ContactName = "Alice",
            Email = $"lead{Guid.NewGuid():N}@example.com",
            Phone = "1234567890",
            EstimatedValue = 1000m,
            Notes = "Seed lead",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };

        db.Leads.Add(lead);
        db.SaveChanges();
        return lead.Id;
    }
}
