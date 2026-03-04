using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public class UpsertCustomerPreferenceCommandValidator : AbstractValidator<UpsertCustomerPreferenceCommand>
{
    private readonly IChannelCountryRepository _channelCountryRepository;
    private readonly IChannelTypeRepository _channelTypeRepository;
    private readonly ICountryRepository _countryRepository;

    public UpsertCustomerPreferenceCommandValidator(IChannelCountryRepository channelCountryRepository,
        IChannelTypeRepository channelTypeRepository,
        ICountryRepository countryRepository)
    {
        _channelCountryRepository = channelCountryRepository;
        _channelTypeRepository = channelTypeRepository;
        _countryRepository = countryRepository;

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

        return await _channelCountryRepository.IsAllowedAsync(request.ChannelId, request.CountryId.Value, cancellationToken);
    }
}
