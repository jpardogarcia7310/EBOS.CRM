namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public record AddAddressRequest(
    bool IsPrimary,
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
    long CustomerId,
    long CountryId,
    long AddressTypeId
    );