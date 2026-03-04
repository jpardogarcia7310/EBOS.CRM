using EBOS.CRM.Application.Features.CRM.CustomerMerge;
using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandValidator : AbstractValidator<PatchCustomerCommand>
{
    private readonly IValidationCatalogService _validationCatalog;
    private readonly ICountryRepository _countryRepository;

    public PatchCustomerCommandValidator(IValidationCatalogService validationCatalog, ICountryRepository countryRepository)
    {
        _validationCatalog = validationCatalog;
        _countryRepository = countryRepository;

        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CustomerRequest).NotNull();

        When(x => x.CustomerRequest != null, () =>
        {
            RuleFor(x => x.CustomerRequest)
                .Must(r =>
                    r.Code != null ||
                    r.Email != null ||
                    r.Phone != null ||
                    r.StatusId.HasValue)
                .WithMessage("At least one field must be provided.");

            When(x => x.CustomerRequest.Code != null, () =>
            {
                RuleFor(x => x.CustomerRequest.Code!)
                    .NotEmpty().MaximumLength(50);
            });

            When(x => x.CustomerRequest.Email != null, () =>
            {
                RuleFor(x => x.CustomerRequest.Email!)
                    .NotEmpty().MaximumLength(100).EmailAddress();
            });

            When(x => x.CustomerRequest.Phone != null, () =>
            {
                RuleFor(x => x.CustomerRequest.Phone!)
                    .NotEmpty()
                    .MaximumLength(CustomerDedupeValidationRules.MaxPhoneDigits)
                    .Matches(CustomerDedupeValidationRules.DigitsPattern);

                RuleFor(x => x.CustomerRequest)
                    .MustAsync(PhoneMatchesAsync)
                    .WithMessage("Phone does not match the configured mask.");

                When(x => x.CustomerRequest.CountryId.HasValue, () =>
                {
                    RuleFor(x => x.CustomerRequest.CountryId!.Value).GreaterThan(0);
                    RuleFor(x => x.CustomerRequest.CountryId!.Value)
                        .MustAsync(CountryExistsAsync)
                        .WithMessage("CountryId does not exist.");
                });
            });

            When(x => x.CustomerRequest.StatusId.HasValue, () =>
            {
                RuleFor(x => x.CustomerRequest.StatusId!.Value).GreaterThan(0);
            });
        });
    }

    private async Task<bool> PhoneMatchesAsync(global::EBOS.CRM.Contracts.Requests.CRM.Customer.PatchCustomerRequest request,
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




