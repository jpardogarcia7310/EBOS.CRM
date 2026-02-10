using System;

namespace EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;

public record AddTaxInformationAddressRequest(
    long TenantId,
    long TaxInformationId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
