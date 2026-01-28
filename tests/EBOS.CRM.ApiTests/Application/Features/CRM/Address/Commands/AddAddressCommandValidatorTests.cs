using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Commands;

public class AddAddressCommandValidatorTests
{
    private readonly AddAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = BuildValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyStreet_Fails()
    {
        var command = BuildValidCommand() with { AddressRequest = BuildValidRequest() with { Street = "" } };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Street);
    }

    [Fact]
    public void Validate_EmptyExternalNumber_Fails()
    {
        var command = BuildValidCommand() with { AddressRequest = BuildValidRequest() with { ExternalNumber = "" } };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.ExternalNumber);
    }

    [Fact]
    public void Validate_ShortPostalCode_Fails()
    {
        var command = BuildValidCommand() with { AddressRequest = BuildValidRequest() with { PostalCode = "1" } };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.PostalCode);
    }

    [Fact]
    public void Validate_InvalidGoogleMapsUrl_Fails()
    {
        var command = BuildValidCommand() with { AddressRequest = BuildValidRequest() with { GoogleMapsUrl = "http://example.com" } };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.GoogleMapsUrl);
    }

    [Theory]
    [InlineData("91")]
    [InlineData("-91")]
    public void Validate_InvalidLatitude_Fails(string value)
    {
        var command = BuildValidCommand() with { AddressRequest = BuildValidRequest() with { Latitude = value } };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Latitude);
    }

    [Theory]
    [InlineData("181")]
    [InlineData("-181")]
    public void Validate_InvalidLongitude_Fails(string value)
    {
        var command = BuildValidCommand() with { AddressRequest = BuildValidRequest() with { Longitude = value } };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Longitude);
    }

    [Fact]
    public void Validate_InvalidIds_Fail()
    {
        var request = BuildValidRequest() with
        {
            CustomerId = 0,
            CountryId = 0,
            AddressTypeId = 0
        };
        var command = new AddAddressCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.CustomerId);
        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.CountryId);
        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.AddressTypeId);
    }

    private static AddAddressCommand BuildValidCommand() => new(BuildValidRequest());

    private static AddAddressRequest BuildValidRequest() => new(
        IsPrimary: false,
        Street: "Main St",
        ExternalNumber: "123",
        InternalNumber: null,
        BetweenStreet1: null,
        BetweenStreet2: null,
        Neighbourhood: "Center",
        City: "Quito",
        StateOrProvince: "Pichincha",
        PostalCode: "EC17001",
        GoogleMapsUrl: "https://maps.example.com/q",
        Latitude: "0",
        Longitude: "0",
        CustomerId: 1,
        CountryId: 1,
        AddressTypeId: 1
    );
}
