using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Requests.CRM.Service.CaseActivity;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using CaseActivityEntity = EBOS.CRM.Domain.Entities.CRM.CaseActivity;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Service;

public class CaseActivityTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;

    public CaseActivityTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "CaseActivity");
    }

    [Fact]
    public async Task CaseActivity_CRUD_And_ByCase_Filter_Work()
    {
        var caseId = await CreateCaseAsync();

        var addRequest = new AddCaseActivityRequest(1, caseId, "Activity-1", "Desc",
            CaseActivityEntity.StatusOpen);
        var createResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CaseActivity", addRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<CaseActivityResponse>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v{_version}/CaseActivity/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateRequest = new UpdateCaseActivityRequest(created.Id, 1, caseId, "Activity-Updated", "Desc",
            CaseActivityEntity.StatusInProgress);
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/v{_version}/CaseActivity/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var byCaseResponse = await _client.GetAsync(
            $"/api/v{_version}/CaseActivity/by-case/{caseId}?status=InProgress");
        byCaseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await byCaseResponse.Content.ReadItemsAsync<CaseActivityResponse>();
        items.Should().NotBeEmpty();

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/CaseActivity/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddCaseActivity_TenantMismatch_ReturnsBadRequest()
    {
        var caseId = await CreateCaseAsync();

        var addRequest = new AddCaseActivityRequest(2, caseId, "Activity-1", "Desc",
            CaseActivityEntity.StatusOpen);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/CaseActivity", addRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCaseActivity_WhenCaseClosed_ReturnsServerError()
    {
        var caseId = await CreateCaseAsync();
        await CloseCaseAsync(caseId);

        var addRequest = new AddCaseActivityRequest(1, caseId, "Activity-1", "Desc",
            CaseActivityEntity.StatusOpen);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/CaseActivity", addRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetByCaseId_InvalidStatus_ReturnsBadRequest()
    {
        var caseId = await CreateCaseAsync();

        var response = await _client.GetAsync(
            $"/api/v{_version}/CaseActivity/by-case/{caseId}?status=BadStatus");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCaseActivity_TenantMismatch_ReturnsBadRequest()
    {
        var caseId = await CreateCaseAsync();

        var addRequest = new AddCaseActivityRequest(1, caseId, "Activity-1", "Desc",
            CaseActivityEntity.StatusOpen);
        var createResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CaseActivity", addRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<CaseActivityResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCaseActivityRequest(created!.Id, 2, caseId, "Activity-Updated", "Desc",
            CaseActivityEntity.StatusInProgress);
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/v{_version}/CaseActivity/{created.Id}", updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCaseActivity_InvalidStatus_ReturnsBadRequest()
    {
        var caseId = await CreateCaseAsync();

        var addRequest = new AddCaseActivityRequest(1, caseId, "Activity-1", "Desc",
            CaseActivityEntity.StatusOpen);
        var createResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CaseActivity", addRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<CaseActivityResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCaseActivityRequest(created!.Id, 1, caseId, "Activity-Updated", "Desc",
            "BadStatus");
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/v{_version}/CaseActivity/{created.Id}", updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCaseActivity_NotFound_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/CaseActivity/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<long> CreateCaseAsync()
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
        return created!.Id;
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

    private async Task CloseCaseAsync(long caseId)
    {
        var closeRequest = new CloseCaseRequest(1, DateTime.UtcNow);
        var response = await _client.PatchAsJsonAsync($"/api/v{_version}/Case/{caseId}/close", closeRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
