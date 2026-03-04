namespace EBOS.CRM.Api.Options;

public sealed class OperationalReadinessOptions
{
    public const string SectionName = "OperationalReadiness";

    public int OutboxPendingWarningThreshold { get; init; } = 100;
    public int OutboxPendingCriticalThreshold { get; init; } = 500;
    public int OutboxFailedCriticalThreshold { get; init; } = 10;
    public int ConcurrencyFailuresCriticalThreshold { get; init; } = 20;
    public int OutboxDispatchStaleMinutesThreshold { get; init; } = 15;
}
