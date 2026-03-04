using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.CustomerMerge;

public class CustomerMergeControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");

    [Fact]
    public async Task FindDuplicates_ReturnsSuccess()
    {
        var response = await _client.GetAsync(
            $"/api/v{_version}/CustomerMerge/duplicates?tenantId=1&email=duplicate@example.com&pageNumber=1&pageSize=10");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Merge_And_History_Queries_Work()
    {
        var customers = await _client.GetAsync($"/api/v{_customerVersion}/Customer");
        customers.EnsureSuccessStatusCode();
        var list = (await customers.Content.ReadItemsAsync<CustomerResponse>()).ToList();
        Assert.True(list.Count >= 2, "Expected at least two customers for merge test.");

        var byPrefix = list
            .Where(x => !string.IsNullOrWhiteSpace(x.Code))
            .GroupBy(x => x.Code.Split('-', 2)[0], StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() >= 2);

        Assert.NotNull(byPrefix);
        var candidates = byPrefix!.Take(2).ToList();
        var winnerId = candidates[0].Id;
        var mergedId = candidates[1].Id;

        var mergeRequest = new MergeCustomersRequest(
            TenantId: 1,
            WinnerCustomerId: winnerId,
            MergeCustomerIds: [mergedId],
            Reason: "api-test");

        var mergeResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CustomerMerge/merge", mergeRequest);
        mergeResponse.EnsureSuccessStatusCode();

        var byWinner = await _client.GetAsync(
            $"/api/v{_version}/CustomerMerge/history/by-winner/{winnerId}?tenantId=1");
        byWinner.EnsureSuccessStatusCode();

        var byMerged = await _client.GetAsync(
            $"/api/v{_version}/CustomerMerge/history/by-merged/{mergedId}?tenantId=1");
        byMerged.EnsureSuccessStatusCode();
    }
}
