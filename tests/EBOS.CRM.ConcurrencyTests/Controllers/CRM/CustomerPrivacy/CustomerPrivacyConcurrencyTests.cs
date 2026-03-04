using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CustomerPrivacy;

public class CustomerPrivacyConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPrivacy");

    [Fact]
    public async Task CustomerPrivacy_ReadEndpoints_Concurrency_Works()
    {
        await ConcurrencyHelper.AssertEndpointConcurrencyAsync(_client,
            $"/api/v{_version}/CustomerPrivacy/by-customer/1?tenantId=1",
            $"/api/v{_version}/CustomerPrivacy/by-status/PENDING?tenantId=1");
    }
}
