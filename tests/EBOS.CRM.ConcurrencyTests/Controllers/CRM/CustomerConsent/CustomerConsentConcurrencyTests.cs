using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ConcurrencyTests.Fixtures;
using EBOS.CRM.ConcurrencyTests.Infrastructure;

namespace EBOS.CRM.ConcurrencyTests.Controllers.CRM.CustomerConsent;

public class CustomerConsentConcurrencyTests(ConcurrencyWebApplicationFactory<Program> factory)
    : IClassFixture<ConcurrencyWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerConsent");

    [Fact]
    public async Task CustomerConsent_ReadEndpoints_Concurrency_Works()
    {
        await ConcurrencyHelper.AssertEndpointConcurrencyAsync(_client,
            $"/api/v{_version}/CustomerConsent/by-customer/1?tenantId=1");
    }
}
