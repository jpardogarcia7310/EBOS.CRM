using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Quote;

public class QuoteTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Quote");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Quote");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<QuoteResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Quote/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_Quote()
    {
        var opportunityId = EnsureOpportunity();

        var addRequest = new AddQuoteRequest(
            TenantId: 1,
            OpportunityId: opportunityId,
            Status: "Draft",
            ReferenceNumber: $"Q-{Guid.NewGuid():N}".Substring(0, 12),
            SubtotalAmount: 1000m,
            DiscountAmount: 100m,
            TotalAmount: 900m,
            ValidUntil: DateTime.UtcNow.AddDays(30),
            Notes: "Initial quote");

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Quote", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<QuoteResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateQuoteRequest(
            Id: created!.Id,
            TenantId: 1,
            OpportunityId: opportunityId,
            Status: "Updated",
            ReferenceNumber: created.ReferenceNumber,
            SubtotalAmount: 1200m,
            DiscountAmount: 100m,
            TotalAmount: 1100m,
            ValidUntil: created.ValidUntil,
            Notes: "Updated quote");

        var updateResponse = await _client.PutAsJsonAsync($"/api/v{_version}/Quote/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/Quote/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private long EnsureOpportunity()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var existing = db.Opportunities.FirstOrDefault(o => o.TenantId == 1);
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
            Name = $"Opp-{Guid.NewGuid():N}",
            StageId = stageId,
            OwnerUserId = 1,
            Customer = customer,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(10),
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
