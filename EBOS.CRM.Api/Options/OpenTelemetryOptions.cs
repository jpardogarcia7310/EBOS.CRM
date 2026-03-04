namespace EBOS.CRM.Api.Options;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public bool Enabled { get; init; } = false;
    public string ServiceName { get; init; } = "ebos-crm-api";
    public string ServiceVersion { get; init; } = "1.0.0";
    public string? OtlpEndpoint { get; init; }
}

