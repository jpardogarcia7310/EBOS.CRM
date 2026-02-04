namespace EBOS.CRM.Api.Options;

public sealed class OidcOptions
{
    public const string SectionName = "Authentication:Oidc";

    public string? Authority { get; init; }
    public string? Audience { get; init; }
    public bool RequireHttpsMetadata { get; init; } = true;
    public int ClockSkewSeconds { get; init; } = 60;
    public string[]? ValidIssuers { get; init; }
    public string[]? ValidAudiences { get; init; }
}
