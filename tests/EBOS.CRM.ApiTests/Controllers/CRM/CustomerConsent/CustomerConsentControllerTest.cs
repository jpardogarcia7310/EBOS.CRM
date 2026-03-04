using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.CustomerConsent;

public class CustomerConsentControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerConsent");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");

    [Fact]
    public async Task GetByCustomer_ReturnsSuccess()
    {
        var customerId = await ControllerTestHelper.GetFirstIdAsync<CustomerResponse>(
            _client, $"/api/v{_customerVersion}/Customer", x => x.Id);

        var response = await _client.GetAsync(
            $"/api/v{_version}/CustomerConsent/by-customer/{customerId}?tenantId=1");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Add_And_Revoke_Work()
    {
        var customerId = await ControllerTestHelper.GetFirstIdAsync<CustomerResponse>(
            _client, $"/api/v{_customerVersion}/Customer", x => x.Id);

        var add = new AddCustomerConsentRequest(
            TenantId: 1,
            CustomerId: customerId,
            ConsentType: "MARKETING_EMAIL",
            Granted: true,
            GrantedAt: DateTime.UtcNow,
            Source: "api-test",
            ExpiresAt: null);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CustomerConsent", add);
        addResponse.EnsureSuccessStatusCode();
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerConsentResponse>();
        Assert.NotNull(created);

        var revoke = new RevokeCustomerConsentRequest(1, DateTime.UtcNow);
        var revokeResponse = await _client.PatchAsJsonAsync(
            $"/api/v{_version}/CustomerConsent/{created!.Id}/revoke", revoke);
        revokeResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Revoke_NonExisting_ReturnsNotFound()
    {
        var revoke = new RevokeCustomerConsentRequest(1, DateTime.UtcNow);
        var response = await _client.PatchAsJsonAsync(
            $"/api/v{_version}/CustomerConsent/999999/revoke", revoke);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
