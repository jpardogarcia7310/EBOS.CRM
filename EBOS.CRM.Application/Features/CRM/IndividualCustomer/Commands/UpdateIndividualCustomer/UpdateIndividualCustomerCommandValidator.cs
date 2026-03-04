using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public class UpdateIndividualCustomerCommandValidator : AbstractValidator<UpdateIndividualCustomerCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IIdentificationTypeRepository _identificationTypeRepository;
    private readonly IValidationCatalogService _validationCatalog;

    public UpdateIndividualCustomerCommandValidator(ICountryRepository countryRepository,
        IIdentificationTypeRepository identificationTypeRepository,
        IValidationCatalogService validationCatalog)
    {
        _countryRepository = countryRepository;
        _identificationTypeRepository = identificationTypeRepository;
        _validationCatalog = validationCatalog;

        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.IndividualCustomerRequest).NotNull();
        RuleFor(x => x.IndividualCustomerRequest.Code).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.Email).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.Phone).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.FirstName).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.LastName).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.IdentificationNumber).MaximumLength(500);
        RuleFor(x => x.IndividualCustomerRequest.StatusId).GreaterThan(0);
        RuleFor(x => x.IndividualCustomerRequest.IdentificationTypeId).GreaterThan(0);

        RuleFor(x => x.IndividualCustomerRequest.IdentificationTypeId)
            .MustAsync(IdentificationTypeExistsAsync)
            .WithMessage("IdentificationTypeId does not exist.");

        RuleFor(x => x.IndividualCustomerRequest)
            .MustAsync(IdentificationNumberMatchesTypeAsync)
            .WithMessage("IdentificationNumber does not match the configured mask.");

        RuleFor(x => x.IndividualCustomerRequest)
            .MustAsync(PhoneMatchesDefaultAsync)
            .WithMessage("Phone does not match the configured mask.");

        When(x => x.IndividualCustomerRequest.CountryId.HasValue, () =>
        {
            RuleFor(x => x.IndividualCustomerRequest.CountryId!.Value).GreaterThan(0);
            RuleFor(x => x.IndividualCustomerRequest.CountryId!.Value)
                .MustAsync(CountryExistsAsync)
                .WithMessage("CountryId does not exist.");
        });
    }

    private async Task<bool> IdentificationTypeExistsAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _identificationTypeRepository.GetByIdAsync(id, cancellationToken);
        return entity is not null;
    }

    private async Task<bool> IdentificationNumberMatchesTypeAsync(global::EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer.UpdateIndividualCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            return true;
        }

        var type = await _identificationTypeRepository.GetByIdAsync(request.IdentificationTypeId, cancellationToken);
        if (type is null)
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(ValidationRuleKeys.Identification(type.Code),
            cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.IdentificationNumber, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }

    private async Task<bool> PhoneMatchesDefaultAsync(global::EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer.UpdateIndividualCustomerRequest request,
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




