namespace EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;

public record AddBranchOfficeAddressRequest(
    long BranchOfficeId,
    long AddressId,
    bool IsPrimary,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsCurrent
);
