using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Opportunity;

public class OpportunityOutcomeEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task WinLossEndpoints_ReturnSuccess()
    {
        var (opportunityId, stageId) = SeedOpportunity(factory);

        var winResponse = await _client.PostAsJsonAsync($"/api/v2/Opportunity/{opportunityId}/win",
            new WinOpportunityRequest(1, stageId, "Won in integration test"));
        winResponse.EnsureSuccessStatusCode();

        var lossResponse = await _client.PostAsJsonAsync($"/api/v2/Opportunity/{opportunityId}/loss",
            new LossOpportunityRequest(1, stageId, "Lost in integration test"));
        lossResponse.EnsureSuccessStatusCode();

        var dto = await lossResponse.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(dto);
        Assert.Equal(opportunityId, dto.Id);
    }

    private static (long opportunityId, long stageId) SeedOpportunity(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var statusId = db.Statuses.Select(s => s.Id).First();
        var customer = new global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer
        {
            TenantId = 1,
            Code = "CORP-OUT",
            Email = "outcome@contoso.com",
            Phone = "+34 911 000 333",
            StatusId = statusId,
            LegalName = "Outcome Corp",
            TaxIdentification = "B12345678",
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };
        db.CorporateCustomers.Add(customer);

        var stage = new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage
        {
            TenantId = 1,
            Name = "Outcome",
            Order = 10,
            DefaultProbability = 0.5m,
            IsClosed = false,
            IsWon = false
        };
        db.OpportunityStages.Add(stage);
        db.SaveChanges();

        var opportunity = new global::EBOS.CRM.Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = "Outcome Opportunity",
            StageId = stage.Id,
            OwnerUserId = 10,
            CustomerId = customer.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(10),
            Amount = 5000m,
            Probability = 0.5m
        };
        db.Opportunities.Add(opportunity);
        db.SaveChanges();

        return (opportunity.Id, stage.Id);
    }
}
