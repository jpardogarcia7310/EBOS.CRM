using EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;

public class GetChannelTypeByIdQueryValidatorTest
{
    private readonly GetChannelTypeByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetChannelTypeByIdQuery(1));
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetChannelTypeByIdQuery(0));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
