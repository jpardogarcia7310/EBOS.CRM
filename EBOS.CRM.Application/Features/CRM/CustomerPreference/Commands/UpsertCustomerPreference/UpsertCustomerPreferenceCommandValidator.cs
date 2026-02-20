using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public class UpsertCustomerPreferenceCommandValidator : AbstractValidator<UpsertCustomerPreferenceCommand>
{
    private readonly IChannelTypeRepository _channelTypeRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IValidationCatalogService _validationCatalog;

    public UpsertCustomerPreferenceCommandValidator(IChannelTypeRepository channelTypeRepository,
        ICountryRepository countryRepository,
        IValidationCatalogService validationCatalog)
    {
        _channelTypeRepository = channelTypeRepository;
        _countryRepository = countryRepository;
        _validationCatalog = validationCatalog;

        RuleFor(x => x.PreferenceRequest).NotNull();
        When(x => x.PreferenceRequest != null, () =>
        {
            RuleFor(x => x.PreferenceRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.PreferenceRequest.CustomerId).GreaterThan(0);
            RuleFor(x => x.PreferenceRequest.ChannelId).GreaterThan(0);

            RuleFor(x => x.PreferenceRequest.ChannelId)
                .MustAsync(ChannelExistsAndActiveAsync)
                .WithMessage("ChannelId does not exist or is inactive.");

            When(x => x.PreferenceRequest.CountryId.HasValue, () =>
            {
                RuleFor(x => x.PreferenceRequest.CountryId!.Value).GreaterThan(0);
                RuleFor(x => x.PreferenceRequest.CountryId!.Value)
                    .MustAsync(CountryExistsAsync)
                    .WithMessage("CountryId does not exist.");

                RuleFor(x => x.PreferenceRequest)
                    .MustAsync(ChannelAllowedForCountryAsync)
                    .WithMessage("ChannelId is not allowed for the provided country.");
            });
        });
    }

    private async Task<bool> ChannelExistsAndActiveAsync(long channelId, CancellationToken cancellationToken)
    {
        var channel = await _channelTypeRepository.GetByIdAsync(channelId, cancellationToken);
        return channel is not null && channel.IsActive;
    }

    private async Task<bool> CountryExistsAsync(long countryId, CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
        return country is not null;
    }

    private async Task<bool> ChannelAllowedForCountryAsync(global::EBOS.CRM.Contracts.Requests.CRM.CustomerPreference.UpsertCustomerPreferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.CountryId.HasValue || request.CountryId.Value <= 0)
        {
            return true;
        }

        var country = await _countryRepository.GetByIdAsync(request.CountryId.Value, cancellationToken);
        var iso2 = country?.Iso31661A2Code;
        if (string.IsNullOrWhiteSpace(iso2))
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(ValidationRuleKeys.Channel(iso2.ToUpperInvariant()), cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.ChannelId.ToString(), pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }
}
