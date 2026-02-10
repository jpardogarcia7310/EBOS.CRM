namespace EBOS.CRM.Application.Contracts.Responses;


public record AddressTypeResponse(
    long Id,
    string Code,
    string Description,
    string? Category,
    bool AllowsMultiple,
    bool RequiresPrimary
);
