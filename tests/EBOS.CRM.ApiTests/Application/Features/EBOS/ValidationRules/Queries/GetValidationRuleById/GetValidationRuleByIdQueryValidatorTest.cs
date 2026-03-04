using EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;

public class GetValidationRuleByIdQueryValidatorTest
{
    private readonly GetValidationRuleByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidId_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetValidationRuleByIdQuery(1));
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetValidationRuleByIdQuery(0));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
