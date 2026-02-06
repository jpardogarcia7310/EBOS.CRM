using System.Security.Claims;
using EBOS.CRM.Api.Constants;

namespace EBOS.CRM.Api.Services;

public sealed class HttpContextCurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    public long UserId
    {
        get
        {
            var httpContext = accessor.HttpContext;
            if (httpContext == null)
            {
                return 0;
            }

            var claim = httpContext.User.FindFirstValue("sub")
                        ?? httpContext.User.FindFirstValue("userId")
                        ?? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return long.TryParse(claim, out var userId) ? userId : 0;
        }
    }

    public long TenantId
    {
        get
        {
            var httpContext = accessor.HttpContext;
            if (httpContext == null)
            {
                return 0;
            }

            var claim = httpContext.User.FindFirstValue("tenantId")
                        ?? httpContext.User.FindFirstValue("tenant_id")
                        ?? httpContext.Request.Headers[HeaderNames.TenantId].FirstOrDefault();

            return long.TryParse(claim, out var tenantId) ? tenantId : 0;
        }
    }

    public string CorrelationId
    {
        get
        {
            var httpContext = accessor.HttpContext;
            if (httpContext == null)
            {
                return Guid.NewGuid().ToString("D");
            }

            if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value) &&
                value is string stored && !string.IsNullOrWhiteSpace(stored))
            {
                return stored;
            }

            var header = httpContext.Request.Headers[CorrelationIdMiddleware.HeaderName].FirstOrDefault();
            return string.IsNullOrWhiteSpace(header) ? Guid.NewGuid().ToString("D") : header;
        }
    }
}

