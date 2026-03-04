using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Middleware;

public class TenantResolutionMiddleware(RequestDelegate next, IOptions<TenantResolutionOptions> options)
{
    private readonly TenantResolutionOptions _options = options.Value;

    public async Task Invoke(HttpContext context)
    {
        if (!context.Items.ContainsKey(TenantContextKeys.TenantId))
        {
            if (!TryResolveTenantId(context, out var tenantId))
            {
                await next(context);
                return;
            }

            context.Items[TenantContextKeys.TenantId] = tenantId;
        }

        await next(context);
    }

    private bool TryResolveTenantId(HttpContext context, out long tenantId)
    {
        tenantId = 0;

        if (_options.EnableHeader &&
            TryResolveFromHeader(context.Request.Headers, _options.HeaderName, out tenantId))
        {
            return true;
        }

        if (_options.EnableSubdomain &&
            TryResolveFromSubdomain(context.Request.Host.Host, _options.SubdomainPrefix, out tenantId))
        {
            return true;
        }

        tenantId = 0;
        return false;
    }

    private static bool TryResolveFromHeader(IHeaderDictionary headers, string headerName, out long tenantId)
    {
        tenantId = 0;
        if (string.IsNullOrWhiteSpace(headerName))
        {
            return false;
        }

        var raw = headers[headerName].FirstOrDefault();
        return long.TryParse(raw, out tenantId) && tenantId > 0;
    }

    private static bool TryResolveFromSubdomain(string? host, string? prefix, out long tenantId)
    {
        tenantId = 0;
        if (string.IsNullOrWhiteSpace(host) || host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var dotIndex = host.IndexOf('.');
        var label = dotIndex > 0 ? host[..dotIndex] : host;
        if (string.IsNullOrWhiteSpace(label))
        {
            return false;
        }

        if (long.TryParse(label, out tenantId) && tenantId > 0)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(prefix))
        {
            return false;
        }

        if (!label.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var numeric = label[prefix.Length..];
        return long.TryParse(numeric, out tenantId) && tenantId > 0;
    }
}
