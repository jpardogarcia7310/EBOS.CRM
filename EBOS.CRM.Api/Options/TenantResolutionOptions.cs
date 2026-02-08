using EBOS.CRM.Api.Constants;

namespace EBOS.CRM.Api.Options;

public sealed class TenantResolutionOptions
{
    public const string SectionName = "TenantResolution";

    public bool EnableHeader { get; init; } = true;
    public bool EnableSubdomain { get; init; } = true;
    public string HeaderName { get; init; } = HeaderNames.TenantId;
    public string SubdomainPrefix { get; init; } = "tenant";
}
