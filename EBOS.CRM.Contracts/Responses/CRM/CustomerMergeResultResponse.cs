namespace EBOS.CRM.Contracts.Responses.CRM;

public record CustomerMergeResultResponse(
    long WinnerCustomerId,
    IReadOnlyCollection<long> MergedCustomerIds,
    string Status
);
