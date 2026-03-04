using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using System.Net.Http.Json;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.Service.Case;

public class CaseConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Case");

    [Fact]
    public async Task Case_ReadConcurrency_Works()
    {
        var baseUrl = $"/api/v{_version}/Case";
        var id = await CreateCaseAsync();

        await ConcurrencyHelper.AssertReadConcurrencyAsync(_client, baseUrl, id);
    }

    [Fact]
    public async Task Case_WriteConcurrency_Returns_NoServerErrors()
    {
        var baseUrl = $"/api/v{_version}/Case";
        var id = await CreateCaseAsync();

        var payloads = await ConcurrencyPayloads.GetPayloadFactoriesAsync(_client, _version, "Case");

        await ConcurrencyHelper.AssertWriteConcurrencyAsync(_client, baseUrl, id, payloads);
    }

    private async Task<long> CreateCaseAsync()
    {
        var addSla = new AddSlaRequest(1, $"SLA-{Guid.NewGuid():N}", 120, 60, null, null, true);
        var slaResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Sla", addSla);
        slaResponse.EnsureSuccessStatusCode();
        var sla = await slaResponse.Content.ReadFromJsonAsync<SlaResponse>();

        var addQueue = new AddQueueRequest(1, $"Queue-{Guid.NewGuid():N}", $"Q{Guid.NewGuid():N}"[..6], true, 10);
        var queueResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Queue", addQueue);
        queueResponse.EnsureSuccessStatusCode();
        var queue = await queueResponse.Content.ReadFromJsonAsync<QueueResponse>();

        var addCase = new AddCaseRequest(
            TenantId: 1,
            Title: $"Case-{Guid.NewGuid():N}",
            Description: "Concurrency case",
            Status: "Open",
            Priority: "Low",
            OwnerUserId: 10,
            QueueId: queue!.Id,
            SlaId: sla!.Id,
            DueAt: DateTime.UtcNow.AddDays(2));
        var caseResponse = await _client.PostAsJsonAsync($"/api/v{_version}/Case", addCase);
        caseResponse.EnsureSuccessStatusCode();
        var created = await caseResponse.Content.ReadFromJsonAsync<CaseResponse>();
        return created!.Id;
    }
}
