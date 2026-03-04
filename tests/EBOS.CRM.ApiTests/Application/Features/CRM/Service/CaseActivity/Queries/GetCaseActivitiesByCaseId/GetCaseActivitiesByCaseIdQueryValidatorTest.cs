using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivitiesByCaseId;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivitiesByCaseId;

public class GetCaseActivitiesByCaseIdQueryValidatorTest
{
    private readonly GetCaseActivitiesByCaseIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidQuery_Passes()
    {
        var query = new GetCaseActivitiesByCaseIdQuery(1, 1, 10, "OPEN", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_InvalidRangeAndStatus_Fails()
    {
        var query = new GetCaseActivitiesByCaseIdQuery(0, 0, 0, "BAD", DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.CaseId);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
        result.ShouldHaveValidationErrorFor(x => x.Status);
        result.ShouldHaveValidationErrorFor(x => x.To);
    }
}
