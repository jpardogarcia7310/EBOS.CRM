namespace EBOS.CRM.Contracts.Requests.CRM.BranchOfficeAddress;

public record AddBranchOfficeAddressRequest(
    long TenantId,
    long BranchOfficeId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
