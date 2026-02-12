using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Service;

public class QueueTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;

    public QueueTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "Queue");
    }

    [Fact]
    public async Task Queue_CRUD_And_Patches_Work()
    {
        var request = new AddQueueRequest(1, $"Queue-{Guid.NewGuid():N}", $"Q{Guid.NewGuid():N}"[..6], true, null);
        var createResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Queue", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<QueueResponse>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v{_version}/Queue/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateRequest = new UpdateQueueRequest(created.Id, 1, "Queue-Updated", "QUPD", true, null);
        var updateResponse = await _client.PutAsJsonAsync($"/api/v{_version}/Queue/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var toggleRequest = new ToggleQueueRequest(1, false);
        var toggleResponse = await _client.PatchAsJsonAsync($"/api/v{_version}/Queue/{created.Id}/toggle", toggleRequest);
        toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var defaultOwnerRequest = new AssignQueueDefaultOwnerRequest(1, 99);
        var defaultOwnerResponse = await _client.PatchAsJsonAsync(
            $"/api/v{_version}/Queue/{created.Id}/default-owner", defaultOwnerRequest);
        defaultOwnerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
