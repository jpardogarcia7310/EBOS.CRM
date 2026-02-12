namespace EBOS.CRM.Contracts.Requests.CRM.Address;

public record AddAddressRequest(
    long TenantId,
    string Street,
    string ExternalNumber,
    string? InternalNumber,
    string? BetweenStreet1,
    string? BetweenStreet2,
    string? Neighbourhood,
    string City,
    string StateOrProvince,
    string PostalCode,
    string? GoogleMapsUrl,
    string? Latitude,
    string? Longitude,
    long CountryId,
    long AddressTypeId
);