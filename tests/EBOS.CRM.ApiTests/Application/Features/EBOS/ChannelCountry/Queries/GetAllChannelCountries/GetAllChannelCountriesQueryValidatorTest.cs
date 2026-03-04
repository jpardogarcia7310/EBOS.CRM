using EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetAllChannelCountries;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelCountry.Queries.GetAllChannelCountries;

public class GetAllChannelCountriesQueryValidatorTest
{
    private readonly GetAllChannelCountriesQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidPaging_Passes()
    {
        var query = new GetAllChannelCountriesQuery(1, 25);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var query = new GetAllChannelCountriesQuery(0, 25);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorCode("VAL_PAGE_POSITIVE")
            .WithErrorMessage("PageNumber must be greater than 0.");
    }

    [Fact]
    public async Task Validate_InvalidPageSize_Fails()
    {
        var query = new GetAllChannelCountriesQuery(1, 0);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorCode("VAL_SIZE_POSITIVE")
            .WithErrorMessage("PageSize must be greater than 0.");
    }
}
