using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Features.CRM.CustomerMerge;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;

public class AddCorporateCustomerCommandValidator : AbstractValidator<AddCorporateCustomerCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ValidationCatalogOptions _options;

    public AddCorporateCustomerCommandValidator(ICountryRepository countryRepository,
        IOptions<ValidationCatalogOptions> options)
    {
        _countryRepository = countryRepository;
        _options = options.Value ?? new ValidationCatalogOptions();

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
        });
    }

    private async Task<bool> TaxIdentificationMatchesCountryAsync(global::EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer.AddCorporateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TaxIdentification))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(_options.DefaultCountryIso2))
        {
            return true;
        }

        var country = await _countryRepository.GetAllAsync(cancellationToken);
        var match = country.FirstOrDefault(c =>
            string.Equals(c.Iso31661A2Code, _options.DefaultCountryIso2, StringComparison.OrdinalIgnoreCase));
        if (match is null)
        {
            return true;
        }

        if (!_options.TaxIdPatternsByCountry.TryGetValue(match.Iso31661A2Code, out var pattern) || string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.TaxIdentification, pattern, RegexOptions.CultureInvariant);
    }
}




