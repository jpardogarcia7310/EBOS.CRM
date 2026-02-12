using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Service;

public class SlaTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _version;

    public SlaTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _version = ApiVersionHelper.GetLatestVersion(factory, "Sla");
    }

    [Fact]
    public async Task Sla_CRUD_And_Toggle_Works()
    {
        var request = new AddSlaRequest(1, $"SLA-{Guid.NewGuid():N}", 60, 30, null, null, true);
        var createResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Sla", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<SlaResponse>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v{_version}/Sla/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateRequest = new UpdateSlaRequest(created.Id, 1, $"SLA-{Guid.NewGuid():N}", 120, 60, null, null, true);
        var updateResponse = await _client.PutAsJsonAsync($"/api/v{_version}/Sla/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var toggleRequest = new ToggleSlaRequest(1, false);
        var toggleResponse = await _client.PatchAsJsonAsync($"/api/v{_version}/Sla/{created.Id}/toggle", toggleRequest);
        toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
