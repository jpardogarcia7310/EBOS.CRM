namespace EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;

public sealed record RunCustomerPrivacyRetentionRequest(
    long TenantId,
    bool DryRun = true,
    int? RetentionDays = null,
    int? BatchSize = null);
