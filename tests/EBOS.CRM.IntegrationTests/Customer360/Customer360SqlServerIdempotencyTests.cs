using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Customer360;

public sealed class Customer360SqlServerIdempotencyTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _tenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _customerPrivacyVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerPrivacy");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [RequiresTestcontainersFact]
    public async Task CustomerPrivacy_ExecuteTwice_IsIdempotent_OnSqlServer()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var email = $"sql-idem-{Guid.NewGuid():N}@example.com";
        var createResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerVersion}/Customer",
            new AddCustomerRequest(1, $"C-{Guid.NewGuid():N}"[..12], email, "34610000000", statusId));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();

        var registerResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerPrivacyVersion}/CustomerPrivacy/register",
            new RegisterCustomerPrivacyRequestRequest(
                TenantId: 1,
                CustomerId: customer!.Id,
                RequestType: "ANONYMIZE",
                Reason: "sql server idempotency",
                ExecuteNow: false));
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var privacy = await registerResponse.Content.ReadFromJsonAsync<CustomerPrivacyRequestResponse>();
        privacy.Should().NotBeNull();

        var execute1 = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerPrivacyVersion}/CustomerPrivacy/{privacy!.Id}/execute",
            new ExecuteCustomerPrivacyRequestRequest(1));
        execute1.StatusCode.Should().Be(HttpStatusCode.OK);
        var first = await execute1.Content.ReadFromJsonAsync<CustomerPrivacyRequestResponse>();
        first.Should().NotBeNull();
        first!.Status.Should().Be("COMPLETED");

        var execute2 = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerPrivacyVersion}/CustomerPrivacy/{privacy.Id}/execute",
            new ExecuteCustomerPrivacyRequestRequest(1));
        execute2.StatusCode.Should().Be(HttpStatusCode.OK);
        var second = await execute2.Content.ReadFromJsonAsync<CustomerPrivacyRequestResponse>();
        second.Should().NotBeNull();
        second!.Status.Should().Be("COMPLETED");
    }

}
