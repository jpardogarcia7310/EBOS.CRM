using System;

namespace EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;

public record UpdateTaxInformationAddressRequest(
    long TaxInformationId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
