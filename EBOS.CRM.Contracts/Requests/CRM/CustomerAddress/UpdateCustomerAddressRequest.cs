namespace EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;

public record UpdateCustomerAddressRequest(
    long TenantId,
    long CustomerId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
