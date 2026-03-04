using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Opportunity;

public class OpportunityValidationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Opportunity");

    [Fact]
    public async Task GetById_Returns_404_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Opportunity/-1");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Returns_400_WhenRequestIsInvalid()
    {
        var request = new AddOpportunityRequest(
            TenantId: 1,
            Name: "",
            StageId: 0,
            OwnerUserId: 0,
            CustomerId: 0,
            ExpectedCloseDate: null,
            Amount: -10m,
            Probability: 2m,
            Source: new string('x', 101),
            SourceLeadId: null);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Opportunity", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_Returns_400_WhenRequestIsInvalid()
    {
        var request = new UpdateOpportunityRequest(
            Id: 0,
            TenantId: 1,
            Name: "",
            StageId: 0,
            OwnerUserId: 0,
            CustomerId: 0,
            ExpectedCloseDate: null,
            Amount: -10m,
            Probability: 2m,
            Source: new string('x', 101),
            SourceLeadId: null,
            CloseReason: new string('x', 501));

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/Opportunity/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
