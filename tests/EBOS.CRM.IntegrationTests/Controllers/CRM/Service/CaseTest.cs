using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Service;

public class CaseTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;

    public CaseTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "Case");
    }

    [Fact]
    public async Task Case_CRUD_And_Close_Works()
    {
        var slaId = await CreateSlaAsync();
        var queueId = await CreateQueueAsync();

        var addRequest = new AddCaseRequest(
            TenantId: 1,
            Title: "Case-1",
            Description: "Desc",
            Status: "Open",
            Priority: "Low",
            OwnerUserId: 10,
            QueueId: queueId,
            SlaId: slaId,
            DueAt: null);

        var createResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Case", addRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<CaseResponse>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v{_version}/Case/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateRequest = new UpdateCaseRequest(
            Id: created.Id,
            TenantId: 1,
            Title: "Case-Updated",
            Description: "Desc",
            Status: "InProgress",
            Priority: "Low",
            OwnerUserId: 10,
            QueueId: queueId,
            SlaId: slaId,
            DueAt: null);

        var updateResponse = await _client.PutAsJsonAsync($"/api/v{_version}/Case/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var closeRequest = new CloseCaseRequest(1, DateTime.UtcNow);
        var closeResponse = await _client.PatchAsJsonAsync($"/api/v{_version}/Case/{created.Id}/close", closeRequest);
        closeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/Case/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<long> CreateSlaAsync()
    {
        var request = new AddSlaRequest(1, $"SLA-{Guid.NewGuid():N}", 60, 30, null, null, true);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Sla", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<SlaResponse>();
        return created!.Id;
    }

    private async Task<long> CreateQueueAsync()
    {
        var request = new AddQueueRequest(1, $"Queue-{Guid.NewGuid():N}", $"Q{Guid.NewGuid():N}"[..6], true, null);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Queue", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<QueueResponse>();
        return created!.Id;
    }
}
