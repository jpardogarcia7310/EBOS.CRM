namespace EBOS.CRM.Contracts.Responses.EBOS;

public record AddressTypeResponse(
    long Id,
    string Code,
    string Description,
    string? Category,
    bool AllowsMultiple,
    bool RequiresPrimary
);
