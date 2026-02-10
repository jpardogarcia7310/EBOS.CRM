using System;

namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;

public record UpdateBranchOfficeAddressRequest(
    long TenantId,
    long BranchOfficeId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
