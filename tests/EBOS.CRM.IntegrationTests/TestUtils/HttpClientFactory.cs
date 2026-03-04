using EBOS.CRM.Api.Constants;
using EBOS.CRM.IntegrationTests.Infrastructure;

namespace EBOS.CRM.IntegrationTests.TestUtils;

public static class HttpClientFactory
{
    public static HttpClient CreateClientWithTenant(CustomWebApplicationFactory factory, long tenantId)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        client.DefaultRequestHeaders.Add(HeaderNames.TenantId, tenantId.ToString());
        return client;
    }
}
