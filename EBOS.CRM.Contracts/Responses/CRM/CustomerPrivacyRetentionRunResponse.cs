namespace EBOS.CRM.Contracts.Responses.CRM;

public sealed record CustomerPrivacyRetentionRunResponse(
    long TenantId,
    bool DryRun,
    int RetentionDays,
    int BatchSize,
    DateTime CutoffUtc,
    int Candidates,
    int Affected);
