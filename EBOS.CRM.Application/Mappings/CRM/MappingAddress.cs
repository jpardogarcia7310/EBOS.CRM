using System.Globalization;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingAddress : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Address -> AddressResponse
        config.NewConfig<Address, AddressResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
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
            .Map(dest => dest.Latitude,
                src => src.Latitude.HasValue ?
                    src.Latitude.Value.ToString(CultureInfo.InvariantCulture) : null)
            .Map(dest => dest.Longitude,
                src => src.Longitude.HasValue ?
                    src.Longitude.Value.ToString(CultureInfo.InvariantCulture) : null)
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.AddressTypeId, src => src.AddressTypeId)
            .Map(dest => dest.Active, src => !src.Erased);

        // AddressResponse -> Address
        config.NewConfig<AddressResponse, Address>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TenantId, src => src.TenantId)
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
            .Map(dest => dest.Latitude, src => ParseNullableDecimal(src.Latitude))
            .Map(dest => dest.Longitude, src => ParseNullableDecimal(src.Longitude))
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.AddressTypeId, src => src.AddressTypeId)
            .Map(dest => dest.Erased, src => !src.Active)
            .Ignore(dest => dest.Country)
            .Ignore(dest => dest.AddressType)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.BranchOfficeAddresses)
            .Ignore(dest => dest.TaxInformationAddresses);

        // AddAddressRequest -> Address
        config.NewConfig<AddAddressRequest, Address>()
            .Map(dest => dest.TenantId, src => src.TenantId)
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
            .Map(dest => dest.Latitude, src => ParseNullableDecimal(src.Latitude))
            .Map(dest => dest.Longitude, src => ParseNullableDecimal(src.Longitude))
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.AddressTypeId, src => src.AddressTypeId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Country)
            .Ignore(dest => dest.AddressType)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.BranchOfficeAddresses)
            .Ignore(dest => dest.TaxInformationAddresses);

        // UpdateAddressRequest -> Address
        config.NewConfig<UpdateAddressRequest, Address>()
            .Map(dest => dest.TenantId, src => src.TenantId)
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
            .Map(dest => dest.Latitude, src => ParseNullableDecimal(src.Latitude))
            .Map(dest => dest.Longitude, src => ParseNullableDecimal(src.Longitude))
            .Map(dest => dest.CountryId, src => src.CountryId)
            .Map(dest => dest.AddressTypeId, src => src.AddressTypeId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Country)
            .Ignore(dest => dest.AddressType)
            .Ignore(dest => dest.CustomerAddresses)
            .Ignore(dest => dest.BranchOfficeAddresses)
            .Ignore(dest => dest.TaxInformationAddresses);
    }

    private static decimal? ParseNullableDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return decimal.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
    }
}


