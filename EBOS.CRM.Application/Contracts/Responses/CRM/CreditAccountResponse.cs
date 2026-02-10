namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CreditAccountResponse(
    long Id,
    long TenantId,
    decimal MaxAmount,
    decimal UsedAmount,
    decimal AvailableAmount,
    long CustomerId,
    bool Active
);
