using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Quote;

public class QuoteDecisionEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ApproveRejectEndpoints_ReturnUpdatedQuote()
    {
        var quoteId = SeedQuote(factory);

        var approveResponse = await _client.PostAsJsonAsync($"/api/v2/Quote/{quoteId}/approve",
            new ApproveQuoteRequest(1, "Approved in integration test", null));
        approveResponse.EnsureSuccessStatusCode();

        var rejectResponse = await _client.PostAsJsonAsync($"/api/v2/Quote/{quoteId}/reject",
            new RejectQuoteRequest(1, "Rejected in integration test", null));
        rejectResponse.EnsureSuccessStatusCode();

        var dto = await rejectResponse.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(dto);
        Assert.Equal("Rejected", dto.Status);
    }

    private static long SeedQuote(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var statusId = db.Statuses.Select(s => s.Id).First();
        var customer = new global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer
        {
            TenantId = 1,
            Code = "CORP-QT",
            Email = "quote@contoso.com",
            Phone = "+34 911 000 444",
            StatusId = statusId,
            LegalName = "Quote Corp",
            TaxIdentification = "B87654321",
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };
        db.CorporateCustomers.Add(customer);

        var stage = new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage
        {
            TenantId = 1,
            Name = "Quote Stage",
            Order = 20,
            DefaultProbability = 0.4m,
            IsClosed = false,
            IsWon = false
        };
        db.OpportunityStages.Add(stage);
        db.SaveChanges();

        var opportunity = new global::EBOS.CRM.Domain.Entities.CRM.Opportunity
        {
            TenantId = 1,
            Name = "Quote Opportunity",
            StageId = stage.Id,
            OwnerUserId = 10,
            CustomerId = customer.Id,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(15),
            Amount = 7000m,
            Probability = 0.4m
        };
        db.Opportunities.Add(opportunity);
        db.SaveChanges();

        var quote = new global::EBOS.CRM.Domain.Entities.CRM.Quote
        {
            TenantId = 1,
            OpportunityId = opportunity.Id,
            Status = "Draft",
            ReferenceNumber = "Q-IT-1",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m,
            Notes = "Integration quote"
        };
        db.Quotes.Add(quote);
        db.SaveChanges();

        return quote.Id;
    }
}
