namespace EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;

public record AddCustomerAddressRequest(
    long TenantId,
    long CustomerId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
