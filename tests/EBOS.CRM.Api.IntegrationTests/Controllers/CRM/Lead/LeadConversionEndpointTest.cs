using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadConversionEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task LeadConversion_ReturnsStatus()
    {
        var leadId = SeedLead(factory);

        var response = await _client.GetAsync($"/api/v2/Lead/{leadId}/conversion");
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<LeadConversionResponse>();
        Assert.NotNull(dto);
        Assert.Equal(leadId, dto.LeadId);
    }

    private static long SeedLead(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var lead = new global::EBOS.CRM.Domain.Entities.CRM.Lead
        {
            TenantId = 1,
            Source = "Integration",
            Status = "New",
            OwnerUserId = 10,
            CompanyName = "Integration Corp",
            ContactName = "Jane Doe",
            Email = "integration.lead@example.com",
            Phone = "1234567890",
            EstimatedValue = 1000m,
            Notes = "Integration lead"
        };
        db.Leads.Add(lead);
        db.SaveChanges();
        return lead.Id;
    }
}
