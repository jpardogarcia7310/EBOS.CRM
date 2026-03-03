namespace EBOS.CRM.Api.Options;

public sealed class CustomerPrivacyRetentionJobOptions
{
    public const string SectionName = "CustomerPrivacyRetentionJob";
    public bool Enabled { get; init; } = false;
    public int SweepIntervalMinutes { get; init; } = 60;
    public bool DryRun { get; init; } = true;
    public int BatchSize { get; init; } = 500;
    public long SystemUserId { get; init; } = 1;
}
