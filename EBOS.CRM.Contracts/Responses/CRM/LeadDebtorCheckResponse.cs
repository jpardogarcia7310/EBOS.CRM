namespace EBOS.CRM.Contracts.Responses.CRM;

public sealed record LeadDebtorCheckResponse(
    bool IsDebtor,
    long? CustomerId,
    string? CustomerType,
    string? Code,
    string? Name,
    string? Email,
    string? Phone,
    long? StatusId,
    string? Status,
    DateTime? DebtorSince,
    decimal? DebtAmount
);
