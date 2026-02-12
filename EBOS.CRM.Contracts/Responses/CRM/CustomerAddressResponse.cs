namespace EBOS.CRM.Contracts.Responses.CRM;

public record CustomerAddressResponse(
    long Id,
    long TenantId,
    long CustomerId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent,
    bool Active
);
