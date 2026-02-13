namespace EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;

public record MergeCustomersRequest(
    long TenantId,
    long WinnerCustomerId,
    IReadOnlyCollection<long> MergeCustomerIds,
    string? Reason
);
