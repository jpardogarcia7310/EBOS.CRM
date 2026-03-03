namespace EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;

public sealed record RegisterCustomerPrivacyRequestRequest(
    long TenantId,
    long CustomerId,
    string RequestType,
    string? Reason,
    bool ExecuteNow = false);
