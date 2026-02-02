namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record CustomerAddressResponse(
    long Id,
    long CustomerId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent,
    bool Active
);
