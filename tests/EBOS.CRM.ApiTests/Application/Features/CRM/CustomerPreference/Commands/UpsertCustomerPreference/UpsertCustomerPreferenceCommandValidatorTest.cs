using EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public class UpsertCustomerPreferenceCommandValidatorTest
{
    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var validator = BuildValidator(channelActive: true, countryExists: true, allowed: true);
        var command = new UpsertCustomerPreferenceCommand(new UpsertCustomerPreferenceRequest(
            1, 100, 2, true, 57));

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ChannelInactive_Fails()
    {
        var validator = BuildValidator(channelActive: false, countryExists: true, allowed: true);
        var command = new UpsertCustomerPreferenceCommand(new UpsertCustomerPreferenceRequest(
            1, 100, 2, true, null));

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PreferenceRequest.ChannelId);
    }

    [Fact]
    public async Task Validate_ChannelNotAllowedForCountry_Fails()
    {
        var validator = BuildValidator(channelActive: true, countryExists: true, allowed: false);
        var command = new UpsertCustomerPreferenceCommand(new UpsertCustomerPreferenceRequest(
            1, 100, 2, true, 57));

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PreferenceRequest);
    }

    private static UpsertCustomerPreferenceCommandValidator BuildValidator(bool channelActive, bool countryExists, bool allowed)
    {
        var channelCountryRepository = new Mock<IChannelCountryRepository>();
        channelCountryRepository.Setup(r => r.IsAllowedAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allowed);

        var channelTypeRepository = new Mock<IChannelTypeRepository>();
        channelTypeRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(channelActive
                ? new ChannelType { Id = 2, Descripcion = "Email", IsActive = true }
                : new ChannelType { Id = 2, Descripcion = "Email", IsActive = false });

        var countryRepository = new Mock<ICountryRepository>();
        countryRepository.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countryExists ? new Country { Id = 57, Name = "Colombia" } : null);

        return new UpsertCustomerPreferenceCommandValidator(
            channelCountryRepository.Object,
            channelTypeRepository.Object,
            countryRepository.Object);
    }
}

