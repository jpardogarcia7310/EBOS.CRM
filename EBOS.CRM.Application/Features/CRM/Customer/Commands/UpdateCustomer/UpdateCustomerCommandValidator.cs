using EBOS.CRM.Application.Features.CRM.CustomerMerge;
using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    private readonly IValidationCatalogService _validationCatalog;
    private readonly ICountryRepository _countryRepository;

    public UpdateCustomerCommandValidator(IValidationCatalogService validationCatalog, ICountryRepository countryRepository)
    {
        _validationCatalog = validationCatalog;
        _countryRepository = countryRepository;

        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CustomerRequest).NotNull();
        When(x => x.CustomerRequest != null, () =>
        {
            RuleFor(x => x.CustomerRequest.Code).NotEmpty();
            RuleFor(x => x.CustomerRequest.Email).NotEmpty();
            RuleFor(x => x.CustomerRequest.Phone).NotEmpty();
            RuleFor(x => x.CustomerRequest.StatusId).GreaterThan(0);

            RuleFor(x => x.CustomerRequest.Email)
                .MaximumLength(CustomerDedupeValidationRules.MaxEmailLength)
                .EmailAddress();

            RuleFor(x => x.CustomerRequest.Phone)
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
    }

    private async Task<bool> PhoneMatchesAsync(global::EBOS.CRM.Contracts.Requests.CRM.Customer.UpdateCustomerRequest request,
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




