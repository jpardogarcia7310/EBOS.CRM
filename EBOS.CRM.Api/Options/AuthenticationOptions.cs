namespace EBOS.CRM.Api.Options;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public bool UseAuthority { get; set; } = false;
    public string? Authority { get; set; }
    public string? Audience { get; set; }
    public string? MetadataAddress { get; set; }
    public bool RequireHttpsMetadata { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public string? ValidIssuer { get; set; }
    public string[]? ValidIssuers { get; set; }
    public string[]? ValidAudiences { get; set; }
    public string? NameClaimType { get; set; } = "sub";
    public string? RoleClaimType { get; set; } = "roles";
    public int ClockSkewSeconds { get; set; } = 60;
    public string? SigningKey { get; set; }
}
