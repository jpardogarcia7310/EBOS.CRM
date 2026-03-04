namespace EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;

public sealed record RetryCustomerPrivacyRequestRequest(long TenantId, string? Reason = null);
