using EBOS.CRM.Api.Constants;
using EBOS.CRM.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public sealed class TestCurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    public long UserId => 1;
    public string CorrelationId => "integration-test";
    public long TenantId
    {
        get
        {
            var httpContext = accessor.HttpContext;
            if (httpContext == null)
            {
                return 0;
            }

            var header = httpContext.Request.Headers[HeaderNames.TenantId].FirstOrDefault();
            if (long.TryParse(header, out var tenantId) && tenantId > 0)
            {
                return tenantId;
            }

            if (httpContext.Items.TryGetValue(TenantContextKeys.TenantId, out var resolved) &&
                resolved is long resolvedTenant && resolvedTenant > 0)
            {
                return resolvedTenant;
            }

            return 0;
        }
    }
}
