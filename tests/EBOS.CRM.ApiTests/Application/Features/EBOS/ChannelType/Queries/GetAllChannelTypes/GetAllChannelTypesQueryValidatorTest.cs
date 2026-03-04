using EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;

public class GetAllChannelTypesQueryValidatorTest
{
    private readonly GetAllChannelTypesQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllChannelTypesQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllChannelTypesQuery(0, 10));
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }
}
