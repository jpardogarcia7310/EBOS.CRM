using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Features.CRM.Address.Commands.UpdateAddress;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Commands.UpdateAddress;

public class UpdateAddressCommandValidatorTest
{
    private readonly UpdateAddressCommandValidator _validator = CreateValidator();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = BuildValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateAddressCommand(0, BuildValidRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var command = new UpdateAddressCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest);
    }

    [Fact]
    public void Validate_EmptyStreet_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Street = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Street);
    }

    [Fact]
    public void Validate_StreetTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Street = new string('a', 201) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Street);
    }

    [Fact]
    public void Validate_EmptyExternalNumber_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { ExternalNumber = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.ExternalNumber);
    }

    [Fact]
    public void Validate_ExternalNumberTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { ExternalNumber = new string('a', 21) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.ExternalNumber);
    }

    [Fact]
    public void Validate_InternalNumberTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { InternalNumber = new string('a', 21) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.InternalNumber);
    }

    [Fact]
    public void Validate_BetweenStreet1TooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { BetweenStreet1 = new string('a', 201) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.BetweenStreet1);
    }

    [Fact]
    public void Validate_BetweenStreet2TooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { BetweenStreet2 = new string('a', 201) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.BetweenStreet2);
    }

    [Fact]
    public void Validate_NeighbourhoodTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Neighbourhood = new string('a', 201) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Neighbourhood);
    }

    [Fact]
    public void Validate_EmptyCity_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { City = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.City);
    }

    [Fact]
    public void Validate_CityTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { City = new string('a', 151) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.City);
    }

    [Fact]
    public void Validate_EmptyState_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { StateOrProvince = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.StateOrProvince);
    }

    [Fact]
    public void Validate_StateTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { StateOrProvince = new string('a', 151) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.StateOrProvince);
    }

    [Fact]
    public void Validate_ShortPostalCode_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { PostalCode = "1" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.PostalCode);
    }

    [Fact]
    public void Validate_EmptyPostalCode_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { PostalCode = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.PostalCode);
    }

    [Fact]
    public void Validate_PostalCodeTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { PostalCode = new string('a', 21) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.PostalCode);
    }

    [Fact]
    public void Validate_InvalidGoogleMapsUrl_Fails()
    {
        var command = new UpdateAddressCommand(1,
            BuildValidRequest() with { GoogleMapsUrl = "https://example.com" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.GoogleMapsUrl);
    }

    [Fact]
    public void Validate_GoogleMapsUrlTooLong_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { GoogleMapsUrl = $"https://maps.{new string('a', 490)}" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.GoogleMapsUrl);
    }

    [Theory]
    [InlineData("91")]
    [InlineData("-91")]
    public void Validate_InvalidLatitude_Fails(string value)
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Latitude = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Latitude);
    }

    [Fact]
    public void Validate_NonNumericLatitude_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Latitude = "nope" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Latitude);
    }

    [Theory]
    [InlineData("181")]
    [InlineData("-181")]
    public void Validate_InvalidLongitude_Fails(string value)
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Longitude = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Longitude);
    }

    [Fact]
    public void Validate_NonNumericLongitude_Fails()
    {
        var command = new UpdateAddressCommand(1, BuildValidRequest() with { Longitude = "bad" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.Longitude);
    }

    [Fact]
    public void Validate_InvalidIds_Fail()
    {
        var request = BuildValidRequest() with
        {
            CountryId = 0,
            AddressTypeId = 0
        };
        var command = new UpdateAddressCommand(1, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.CountryId);
        result.ShouldHaveValidationErrorFor(x => x.AddressRequest.AddressTypeId);
    }

    private static UpdateAddressCommand BuildValidCommand() => new(1, BuildValidRequest());

    private static UpdateAddressRequest BuildValidRequest() => new(
            TenantId: 1,
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
        CountryId: 1,
        AddressTypeId: 1
    );

    private static UpdateAddressCommandValidator CreateValidator()
    {
        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Country { Id = 1, Iso31661A2Code = "EC", Name = "Ecuador", CreatedAt = DateTime.UtcNow, CreatedBy = 1, Currency = "USD", CurrencyCode = "USD", Domain = ".ec", InternationalPhoneCode = "593", Iso31661A3Code = "ECU", Iso31661NumCode = "218" });

        var addressTypeRepo = new Mock<IAddressTypeRepository>();
        addressTypeRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AddressType { Id = 1, Code = "HOME", Description = "Home", CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = null, UpdatedBy = null });

        var options = global::Microsoft.Extensions.Options.Options.Create(new ValidationCatalogOptions());
        return new UpdateAddressCommandValidator(countryRepo.Object, addressTypeRepo.Object, options);
    }
}


