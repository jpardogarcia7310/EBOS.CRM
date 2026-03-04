using EBOS.CRM.Application.Features.CRM.CustomerMerge;
using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;

public class AddCorporateCustomerCommandValidator : AbstractValidator<AddCorporateCustomerCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IValidationCatalogService _validationCatalog;

    public AddCorporateCustomerCommandValidator(ICountryRepository countryRepository,
        IValidationCatalogService validationCatalog)
    {
        _countryRepository = countryRepository;
        _validationCatalog = validationCatalog;

        RuleFor(x => x.CorporateCustomerRequest).NotNull();
        When(x => x.CorporateCustomerRequest != null, () =>
        {
            RuleFor(x => x.CorporateCustomerRequest.Code).NotEmpty();
            RuleFor(x => x.CorporateCustomerRequest.Email).NotEmpty();
            RuleFor(x => x.CorporateCustomerRequest.Phone).NotEmpty();
            RuleFor(x => x.CorporateCustomerRequest.LegalName).NotEmpty();
            RuleFor(x => x.CorporateCustomerRequest.TaxIdentification).NotEmpty();
            RuleFor(x => x.CorporateCustomerRequest.StatusId).GreaterThan(0);

            RuleFor(x => x.CorporateCustomerRequest.Email)
                .MaximumLength(CustomerDedupeValidationRules.MaxEmailLength)
                .EmailAddress();

            RuleFor(x => x.CorporateCustomerRequest.Phone)
                .MaximumLength(CustomerDedupeValidationRules.MaxPhoneDigits)
                .Matches(CustomerDedupeValidationRules.DigitsPattern);

            RuleFor(x => x.CorporateCustomerRequest.TaxIdentification)
                .MaximumLength(CustomerDedupeValidationRules.MaxTaxIdLength)
                .Matches(CustomerDedupeValidationRules.AlphanumericPattern);

            RuleFor(x => x.CorporateCustomerRequest)
                .MustAsync(TaxIdentificationMatchesCountryAsync)
                .WithMessage("TaxIdentification does not match the configured country mask.");

            RuleFor(x => x.CorporateCustomerRequest)
                .MustAsync(PhoneMatchesAsync)
                .WithMessage("Phone does not match the configured mask.");

            When(x => x.CorporateCustomerRequest.CountryId.HasValue, () =>
            {
                RuleFor(x => x.CorporateCustomerRequest.CountryId!.Value).GreaterThan(0);
                RuleFor(x => x.CorporateCustomerRequest.CountryId!.Value)
                    .MustAsync(CountryExistsAsync)
                    .WithMessage("CountryId does not exist.");
            });
        });
    }

    private async Task<bool> TaxIdentificationMatchesCountryAsync(global::EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer.AddCorporateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TaxIdentification))
        {
            return true;
        }

        var pattern = await GetTaxIdPatternAsync(request.CountryId, cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.TaxIdentification, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }

    private async Task<bool> PhoneMatchesAsync(global::EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer.AddCorporateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return true;
        }

        var pattern = await GetPhonePatternAsync(request.CountryId, cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.Phone, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }

    private async Task<string?> GetTaxIdPatternAsync(long? countryId, CancellationToken cancellationToken)
    {
        if (countryId.HasValue && countryId.Value > 0)
        {
            var country = await _countryRepository.GetByIdAsync(countryId.Value, cancellationToken);
            var iso2 = country?.Iso31661A2Code;
            if (!string.IsNullOrWhiteSpace(iso2))
            {
                var countryPattern = await _validationCatalog.GetPatternAsync(
                    ValidationRuleKeys.TaxId(iso2.ToUpperInvariant()),
                    cancellationToken);
                if (!string.IsNullOrWhiteSpace(countryPattern))
                {
                    return countryPattern;
                }
            }
        }

        return await _validationCatalog.GetPatternAsync(
            ValidationRuleKeys.TaxId(ValidationRuleKeys.DefaultCountryKey),
            cancellationToken);
    }

    private async Task<string?> GetPhonePatternAsync(long? countryId, CancellationToken cancellationToken)
    {
        if (countryId.HasValue && countryId.Value > 0)
        {
            var country = await _countryRepository.GetByIdAsync(countryId.Value, cancellationToken);
            var iso2 = country?.Iso31661A2Code;
            if (!string.IsNullOrWhiteSpace(iso2))
            {
                var countryPattern = await _validationCatalog.GetPatternAsync(
                    ValidationRuleKeys.Phone(iso2.ToUpperInvariant()),
                    cancellationToken);
                if (!string.IsNullOrWhiteSpace(countryPattern))
                {
                    return countryPattern;
                }
            }
        }

        return await _validationCatalog.GetPatternAsync(
            ValidationRuleKeys.Phone(ValidationRuleKeys.DefaultCountryKey),
            cancellationToken);
    }

    private async Task<bool> CountryExistsAsync(long countryId, CancellationToken cancellationToken)
    {
        var entity = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
        return entity != null;
    }
}




