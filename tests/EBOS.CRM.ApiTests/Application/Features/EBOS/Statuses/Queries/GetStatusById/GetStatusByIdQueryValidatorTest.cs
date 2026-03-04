using EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetStatusById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryValidatorTest
{
    private readonly GetStatusByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        // Arrange
        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public async Task Validate_ZeroId_FailsWithCodeAndMessage()
    {
        // Arrange
        var query = new GetStatusByIdQuery(0);

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.Id)
              .WithErrorCode("VAL_ID_POSITIVE")
              .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public async Task Validate_NegativeId_FailsWithCodeAndMessage()
    {
        // Arrange
        var query = new GetStatusByIdQuery(-5);

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.Id)
              .WithErrorCode("VAL_ID_POSITIVE")
              .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public async Task Validate_MultipleCalls_AreStateless()
    {
        // Arrange
        var queryValid = new GetStatusByIdQuery(10);
        var queryInvalid = new GetStatusByIdQuery(0);

        // Act
        var resultValid = await _validator.TestValidateAsync(queryValid);
        var resultInvalid = await _validator.TestValidateAsync(queryInvalid);

        // Assert
        resultValid.ShouldNotHaveValidationErrorFor(q => q.Id);
        resultInvalid.ShouldHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public async Task Validate_ThreadSafety_UnderParallelInvocations()
    {
        // Arrange
        var queries = new[]
        {
                new GetStatusByIdQuery(1),
                new GetStatusByIdQuery(0),
                new GetStatusByIdQuery(-1),
                new GetStatusByIdQuery(5)
            };

        // Act
        var results = await Task.WhenAll(queries.Select(q => _validator.TestValidateAsync(q)));

        // Assert
        Assert.Contains(results, r => r.IsValid); // At least one valid
        Assert.Contains(results, r => !r.IsValid); // At least one invalid
    }
}




