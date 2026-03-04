using EBOS.CRM.Api.Constants;

namespace EBOS.CRM.ApiTests.Constants;

public class ApiConstantsTest
{
    [Fact]
    public void ApiRouteTemplates_Versioned_IsExpected()
    {
        Assert.Equal("api/v{version:apiVersion}/[controller]", ApiRouteTemplates.Versioned);
    }

    [Fact]
    public void HeaderNames_TenantId_IsExpected()
    {
        Assert.Equal("X-Tenant-Id", HeaderNames.TenantId);
    }

    [Fact]
    public void TenantContextKeys_TenantId_IsExpected()
    {
        Assert.Equal("TenantId", TenantContextKeys.TenantId);
    }
}
