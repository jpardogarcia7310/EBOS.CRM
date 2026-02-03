using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class AddressEntityFactoryTest
{
    public static Address CreateValidAddress(
        string street = "Main St",
        string externalNumber = "123",
        string city = "Quito",
        string stateOrProvince = "Pichincha",
        string postalCode = "EC17001",
        long countryId = 1,
        long addressTypeId = 1)
    {
        return new Address
        {
            Street = street,
            ExternalNumber = externalNumber,
            InternalNumber = null,
            BetweenStreet1 = null,
            BetweenStreet2 = null,
            Neighbourhood = "Center",
            City = city,
            StateOrProvince = stateOrProvince,
            PostalCode = postalCode,
            GoogleMapsUrl = null,
            Latitude = 0,
            Longitude = 0,
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };
    }
    [Fact]
    public void CreateValidAddress_Defaults_AreSet()
    {
        var address = CreateValidAddress();

        Assert.NotNull(address);
        Assert.Equal("Main St", address.Street);
        Assert.Equal("123", address.ExternalNumber);
        Assert.Equal("Quito", address.City);
        Assert.Equal("Pichincha", address.StateOrProvince);
        Assert.Equal("EC17001", address.PostalCode);
        Assert.Equal(1, address.CountryId);
        Assert.Equal(1, address.AddressTypeId);
    }

    [Fact]
    public void CreateValidAddress_CustomValues_AreApplied()
    {
        var address = CreateValidAddress(
            street: "Gran Via",
            externalNumber: "45",
            city: "Madrid",
            stateOrProvince: "Madrid",
            postalCode: "28013",
            countryId: 2,
            addressTypeId: 3);

        Assert.Equal("Gran Via", address.Street);
        Assert.Equal("45", address.ExternalNumber);
        Assert.Equal("Madrid", address.City);
        Assert.Equal("Madrid", address.StateOrProvince);
        Assert.Equal("28013", address.PostalCode);
        Assert.Equal(2, address.CountryId);
        Assert.Equal(3, address.AddressTypeId);
    }
}


