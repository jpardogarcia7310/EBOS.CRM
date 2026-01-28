using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingAddress : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Address, AddressResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.Street, src => src.Street)
            .Map(dest => dest.ExternalNumber, src => src.ExternalNumber)
            .Map(dest => dest.InternalNumber, src => src.InternalNumber)
            .Map(dest => dest.BetweenStreet1, src => src.BetweenStreet1)
            .Map(dest => dest.BetweenStreet2, src => src.BetweenStreet2)
            .Map(dest => dest.Neighbourhood, src => src.Neighbourhood)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.StateOrProvince, src => src.StateOrProvince)
            .Map(dest => dest.PostalCode, src => src.PostalCode)
            .Map(dest => dest.GoogleMapsUrl, src => src.GoogleMapsUrl)
            .Map(dest => dest.Latitude, src => src.Latitude)
            .Map(dest => dest.Longitude, src => src.Longitude)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.AddressTypeId, src => src.AddressTypeId)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddressResponse, Address>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.Street, src => src.Street)
            .Map(dest => dest.ExternalNumber, src => src.ExternalNumber)
            .Map(dest => dest.InternalNumber, src => src.InternalNumber)
            .Map(dest => dest.BetweenStreet1, src => src.BetweenStreet1)
            .Map(dest => dest.BetweenStreet2, src => src.BetweenStreet2)
            .Map(dest => dest.Neighbourhood, src => src.Neighbourhood)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.StateOrProvince, src => src.StateOrProvince)
            .Map(dest => dest.PostalCode, src => src.PostalCode)
            .Map(dest => dest.GoogleMapsUrl, src => src.GoogleMapsUrl)
            .Map(dest => dest.Latitude, src => src.Latitude)
            .Map(dest => dest.Longitude, src => src.Longitude)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.AddressTypeId, src => src.AddressTypeId)
            .Map(dest => dest.Erased, src => !src.Active)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.Country)
            .Ignore(dest => dest.AddressType);
    }
}
