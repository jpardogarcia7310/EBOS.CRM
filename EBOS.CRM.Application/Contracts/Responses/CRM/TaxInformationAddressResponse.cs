using System;

namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record TaxInformationAddressResponse(
    long Id,
    long TaxInformationId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent,
    bool Active
);
