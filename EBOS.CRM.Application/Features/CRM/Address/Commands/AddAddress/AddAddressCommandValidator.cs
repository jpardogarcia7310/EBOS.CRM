using FluentValidation;
using System.Globalization;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(x => x.AddressRequest).NotNull();

        RuleFor(x => x.AddressRequest.Street)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.AddressRequest.ExternalNumber)
            .NotEmpty().MaximumLength(20);

        RuleFor(x => x.AddressRequest.InternalNumber)
            .MaximumLength(20);

        RuleFor(x => x.AddressRequest.BetweenStreet1)
            .MaximumLength(200);

        RuleFor(x => x.AddressRequest.BetweenStreet2)
            .MaximumLength(200);

        RuleFor(x => x.AddressRequest.Neighbourhood)
            .MaximumLength(200);

        RuleFor(x => x.AddressRequest.City)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.AddressRequest.StateOrProvince)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.AddressRequest.PostalCode)
            .NotEmpty().MinimumLength(3).MaximumLength(20);

        RuleFor(x => x.AddressRequest.GoogleMapsUrl)
            .MaximumLength(500)
            .Must(url => string.IsNullOrWhiteSpace(url) || url.StartsWith("https://maps."))
            .WithMessage("GoogleMapsUrl must start with 'https://maps.'");

        RuleFor(x => x.AddressRequest.Latitude)
            .Must(value => string.IsNullOrWhiteSpace(value) || TryParseInRange(value, -90, 90))
            .WithMessage("Latitude must be a valid number between -90 and 90.");

        RuleFor(x => x.AddressRequest.Longitude)
            .Must(value => string.IsNullOrWhiteSpace(value) || TryParseInRange(value, -180, 180))
            .WithMessage("Longitude must be a valid number between -180 and 180.");

        RuleFor(x => x.AddressRequest.CountryId).GreaterThan(0);
        RuleFor(x => x.AddressRequest.AddressTypeId).GreaterThan(0);
    }

    private static bool TryParseInRange(string value, decimal min, decimal max)
    {
        if (!decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return false;

        return parsed >= min && parsed <= max;
    }
}
