using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CustomerPrivacy;

public class CustomerPrivacyEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _tenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPrivacy");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task HappyPath_Register_Execute_And_GetById_Work()
    {
        var customer = await CreateCustomerAsync(1);
        var register = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerPrivacy/register",
            new RegisterCustomerPrivacyRequestRequest(1, customer.Id, "ANONYMIZE", "it", false));
        register.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await register.Content.ReadFromJsonAsync<CustomerPrivacyRequestResponse>();
        created.Should().NotBeNull();

        var execute = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerPrivacy/{created!.Id}/execute",
            new ExecuteCustomerPrivacyRequestRequest(1));
        execute.StatusCode.Should().Be(HttpStatusCode.OK);

        var byId = await _tenant1.GetAsync($"/api/v{_version}/CustomerPrivacy/{created.Id}?tenantId=1");
        byId.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithTenantMismatch_ReturnsBadRequest()
    {
        var customer = await CreateCustomerAsync(1);
        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerPrivacy/register",
            new RegisterCustomerPrivacyRequestRequest(2, customer.Id, "ANONYMIZE", "it", false));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Execute_NonExisting_ReturnsNotFound()
    {
        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerPrivacy/999999/execute",
            new ExecuteCustomerPrivacyRequestRequest(1));
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
                $"privacy-{Guid.NewGuid():N}@example.com",
                "34600000003",
                statusId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }
}
