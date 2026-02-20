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
                .MustAsync(PhoneMatchesDefaultAsync)
                .WithMessage("Phone does not match the configured mask.");
        });
    }

    private async Task<bool> TaxIdentificationMatchesCountryAsync(global::EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer.AddCorporateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TaxIdentification))
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(ValidationRuleKeys.TaxId(ValidationRuleKeys.DefaultCountryKey),
            cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.TaxIdentification, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }

    private async Task<bool> PhoneMatchesDefaultAsync(global::EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer.AddCorporateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(ValidationRuleKeys.Phone(ValidationRuleKeys.DefaultCountryKey),
            cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.Phone, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }
}




