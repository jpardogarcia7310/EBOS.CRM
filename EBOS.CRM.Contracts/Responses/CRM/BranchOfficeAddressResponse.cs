namespace EBOS.CRM.Contracts.Responses.CRM;

public record BranchOfficeAddressResponse(
    long Id,
    long TenantId,
    long BranchOfficeId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent,
    bool Active
);
