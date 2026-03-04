using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Opportunity;

public class OpportunityTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Opportunity");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<OpportunityResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Opportunity/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Works_For_Opportunity()
    {
        var (customerId, stageId) = EnsureOpportunityDependencies();

        var addRequest = new AddOpportunityRequest(
            TenantId: 1,
            Name: $"Opp-{Guid.NewGuid():N}",
            StageId: stageId,
            OwnerUserId: 1,
            CustomerId: customerId,
            ExpectedCloseDate: DateTime.UtcNow.AddDays(10),
            Amount: 1200m,
            Probability: 0.5m,
            Source: "Referral",
            SourceLeadId: null);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<OpportunityResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateOpportunityRequest(
            Id: created!.Id,
            TenantId: 1,
            Name: created.Name,
            StageId: stageId,
            OwnerUserId: 1,
            CustomerId: customerId,
            ExpectedCloseDate: DateTime.UtcNow.AddDays(20),
            Amount: 2500m,
            Probability: 0.7m,
            Source: "Referral",
            SourceLeadId: null,
            CloseReason: null);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/Opportunity/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await updateResponse.Content.ReadFromJsonAsync<OpportunityResponse>();
        updated.Should().NotBeNull();
        updated!.Amount.Should().Be(2500m);
    }

    private (long CustomerId, long StageId) EnsureOpportunityDependencies()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var stageId = db.OpportunityStages.Select(s => s.Id).First();

        var existingCustomer = db.Customers.FirstOrDefault(c => c.TenantId == 1);
        if (existingCustomer != null)
        {
            return (existingCustomer.Id, stageId);
        }

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

        db.Customers.Add(customer);
        db.SaveChanges();

        return (customer.Id, stageId);
    }
}
