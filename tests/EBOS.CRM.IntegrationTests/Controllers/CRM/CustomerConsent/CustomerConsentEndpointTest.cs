using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CustomerConsent;

public class CustomerConsentEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _tenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerConsent");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task HappyPath_Add_GetByCustomer_And_Revoke_Work()
    {
        var customer = await CreateCustomerAsync(1);
        var addResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerConsent",
            new AddCustomerConsentRequest(1, customer.Id, "MARKETING_EMAIL", true, DateTime.UtcNow, "it", null));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerConsentResponse>();
        created.Should().NotBeNull();

        var byCustomer = await _tenant1.GetAsync($"/api/v{_version}/CustomerConsent/by-customer/{customer.Id}?tenantId=1");
        byCustomer.StatusCode.Should().Be(HttpStatusCode.OK);

        var revoke = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_version}/CustomerConsent/{created!.Id}/revoke",
            new RevokeCustomerConsentRequest(1, DateTime.UtcNow));
        revoke.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Add_WithTenantMismatch_ReturnsBadRequest()
    {
        var customer = await CreateCustomerAsync(1);
        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerConsent",
            new AddCustomerConsentRequest(2, customer.Id, "MARKETING_EMAIL", true, DateTime.UtcNow, "it", null));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Revoke_NonExisting_ReturnsNotFound()
    {
        var response = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_version}/CustomerConsent/999999/revoke",
            new RevokeCustomerConsentRequest(1, DateTime.UtcNow));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<CustomerResponse> CreateCustomerAsync(long tenantId)
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerVersion}/Customer",
            new AddCustomerRequest(
                tenantId,
                $"C-{Guid.NewGuid():N}"[..12],
                $"consent-{Guid.NewGuid():N}@example.com",
                "34600000001",
                statusId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }
}
