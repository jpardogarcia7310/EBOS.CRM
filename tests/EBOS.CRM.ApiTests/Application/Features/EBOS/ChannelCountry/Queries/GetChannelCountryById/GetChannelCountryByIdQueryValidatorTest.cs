using EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;

public class GetChannelCountryByIdQueryValidatorTest
{
    private readonly GetChannelCountryByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var query = new GetChannelCountryByIdQuery(1);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_ZeroId_Fails()
    {
        var query = new GetChannelCountryByIdQuery(0);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("Id must be greater than 0.");
    }

    [Fact]
    public async Task Validate_NegativeId_Fails()
    {
        var query = new GetChannelCountryByIdQuery(-1);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("Id must be greater than 0.");
    }
}
