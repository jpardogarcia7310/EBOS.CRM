using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.CustomerPreference;

public class CustomerPreferenceControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPreference");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");

    [Fact]
    public async Task GetByCustomer_ReturnsSuccess()
    {
        var customerId = await ControllerTestHelper.GetFirstIdAsync<CustomerResponse>(
            _client, $"/api/v{_customerVersion}/Customer", x => x.Id);

        var response = await _client.GetAsync(
            $"/api/v{_version}/CustomerPreference/by-customer/{customerId}?tenantId=1");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Upsert_Then_GetByCustomer_Works()
    {
        var customerId = await ControllerTestHelper.GetFirstIdAsync<CustomerResponse>(
            _client, $"/api/v{_customerVersion}/Customer", x => x.Id);

        var request = new UpsertCustomerPreferenceRequest(
            TenantId: 1,
            CustomerId: customerId,
            ChannelId: 1,
            Preferred: true,
            CountryId: 1);

        var upsert = await _client.PutAsJsonAsync($"/api/v{_version}/CustomerPreference", request);
        upsert.EnsureSuccessStatusCode();

        var list = await _client.GetAsync(
            $"/api/v{_version}/CustomerPreference/by-customer/{customerId}?tenantId=1");
        list.EnsureSuccessStatusCode();
    }
}
