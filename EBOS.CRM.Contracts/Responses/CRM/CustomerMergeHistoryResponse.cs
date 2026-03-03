namespace EBOS.CRM.Contracts.Responses.CRM;

public sealed record CustomerMergeHistoryResponse(
    long Id,
    long TenantId,
    long WinnerCustomerId,
    long MergedCustomerId,
    string Reason,
    long CreatedBy,
    DateTime CreatedAt);
