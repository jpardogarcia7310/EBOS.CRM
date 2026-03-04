using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Controllers.CRM.CustomerPrivacy;

public class CustomerPrivacyControllerTest(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPrivacy");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");

    [Fact]
    public async Task Register_Execute_GetById_GetByStatus_Work()
    {
        var customerId = await ControllerTestHelper.GetFirstIdAsync<CustomerResponse>(
            _client, $"/api/v{_customerVersion}/Customer", x => x.Id);

        var registerRequest = new RegisterCustomerPrivacyRequestRequest(
            TenantId: 1,
            CustomerId: customerId,
            RequestType: "ANONYMIZE",
            Reason: "api-test",
            ExecuteNow: false);

        var register = await _client.PostAsJsonAsync($"/api/v{_version}/CustomerPrivacy/register", registerRequest);
        register.EnsureSuccessStatusCode();
        var created = await register.Content.ReadFromJsonAsync<CustomerPrivacyRequestResponse>();
        Assert.NotNull(created);

        var execute = await _client.PostAsJsonAsync(
            $"/api/v{_version}/CustomerPrivacy/{created!.Id}/execute",
            new ExecuteCustomerPrivacyRequestRequest(1));
        execute.EnsureSuccessStatusCode();

        var byId = await _client.GetAsync($"/api/v{_version}/CustomerPrivacy/{created.Id}?tenantId=1");
        byId.EnsureSuccessStatusCode();

        var byStatus = await _client.GetAsync($"/api/v{_version}/CustomerPrivacy/by-status/COMPLETED?tenantId=1");
        byStatus.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Execute_NonExisting_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/v{_version}/CustomerPrivacy/999999/execute",
            new ExecuteCustomerPrivacyRequestRequest(1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
