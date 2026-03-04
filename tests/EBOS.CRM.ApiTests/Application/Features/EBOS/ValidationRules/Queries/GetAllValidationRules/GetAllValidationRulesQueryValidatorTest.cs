using EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetAllValidationRules;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ValidationRules.Queries.GetAllValidationRules;

public class GetAllValidationRulesQueryValidatorTest
{
    private readonly GetAllValidationRulesQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(new GetAllValidationRulesQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidPageNumber_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetAllValidationRulesQuery(0, 10));
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }
}
