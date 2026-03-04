namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditOutboxOptions
{
    public const string SectionName = "AuditOutbox";
    public bool Enabled { get; init; } = true;
    public int MaxAttempts { get; init; } = 10;
    public int BatchSize { get; init; } = 50;
    public int DispatchIntervalSeconds { get; init; } = 30;
}
