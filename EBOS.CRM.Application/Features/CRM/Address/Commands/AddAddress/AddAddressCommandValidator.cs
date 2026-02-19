using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IAddressTypeRepository _addressTypeRepository;
    private readonly IValidationCatalogService _validationCatalog;

    public AddAddressCommandValidator(ICountryRepository countryRepository,
        IAddressTypeRepository addressTypeRepository,
        IValidationCatalogService validationCatalog)
    {
        _countryRepository = countryRepository;
        _addressTypeRepository = addressTypeRepository;
        _validationCatalog = validationCatalog;

        RuleFor(x => x.AddressRequest).NotNull();

        When(x => x.AddressRequest != null, () =>
        {
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

            RuleFor(x => x.AddressRequest.CountryId)
                .MustAsync(CountryExistsAsync)
                .WithMessage("CountryId does not exist.");

            RuleFor(x => x.AddressRequest.AddressTypeId)
                .MustAsync(AddressTypeExistsAsync)
                .WithMessage("AddressTypeId does not exist.");

            RuleFor(x => x.AddressRequest)
                .MustAsync(PostalCodeMatchesCountryAsync)
                .WithMessage("PostalCode does not match the country format.");
        });
    }

    private static bool TryParseInRange(string value, decimal min, decimal max)
    {
        if (!decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            return false;

        return parsed >= min && parsed <= max;
    }

    private async Task<bool> CountryExistsAsync(long countryId, CancellationToken cancellationToken)
    {
        var entity = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
        return entity is not null;
    }

    private async Task<bool> AddressTypeExistsAsync(long addressTypeId, CancellationToken cancellationToken)
    {
        var entity = await _addressTypeRepository.GetByIdAsync(addressTypeId, cancellationToken);
        return entity is not null;
    }

    private async Task<bool> PostalCodeMatchesCountryAsync(global::EBOS.CRM.Contracts.Requests.CRM.Address.AddAddressRequest request,
        CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(request.CountryId, cancellationToken);
        if (country is null)
        {
            return true;
        }

        var iso2 = country.Iso31661A2Code;
        if (string.IsNullOrWhiteSpace(iso2))
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(request.TenantId, ValidationRuleKeys.PostalCode(iso2),
            cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.PostalCode, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }
}




