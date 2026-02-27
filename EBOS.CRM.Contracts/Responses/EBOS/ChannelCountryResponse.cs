namespace EBOS.CRM.Contracts.Responses.EBOS;

public record ChannelCountryResponse(
    long Id,
    long ChannelTypeId,
    string ChannelTypeDescripcion,
    long CountryId,
    string CountryIso2,
    string CountryName,
    string CompositeKey,
    bool IsActive
);
