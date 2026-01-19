using EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;
using EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.Statuses.Queries.GetStatusById;

public class GetEstadoByIdQueryValidatorTest
{
    private readonly GetEstadoByIdQueryValidator _validator;

    public GetEstadoByIdQueryValidatorTest()
    {
        _validator = new GetEstadoByIdQueryValidator();
    }

    [Fact]
    public void Validate_PositiveId_Passes()
    {
        // Arrange
        var query = new GetEstadoByIdQuery(1);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ZeroId_FailsWithCodeAndMessage()
    {
        // Arrange
        var query = new GetEstadoByIdQuery(0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.Id)
              .WithErrorCode("VAL_ID_POSITIVE")
              .WithErrorMessage("El identificador debe ser un número entero positivo mayor que 0.");
    }

    [Fact]
    public void Validate_NegativeId_FailsWithCodeAndMessage()
    {
        // Arrange
        var query = new GetEstadoByIdQuery(-5);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.Id)
              .WithErrorCode("VAL_ID_POSITIVE")
              .WithErrorMessage("El identificador debe ser un número entero positivo mayor que 0.");
    }

    [Fact]
    public void Validate_MultipleCalls_AreStateless()
    {
        // Arrange
        var queryValid = new GetEstadoByIdQuery(10);
        var queryInvalid = new GetEstadoByIdQuery(0);

        // Act
        var resultValid = _validator.TestValidate(queryValid);
        var resultInvalid = _validator.TestValidate(queryInvalid);

        // Assert
        resultValid.ShouldNotHaveValidationErrorFor(q => q.Id);
        resultInvalid.ShouldHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ThreadSafety_UnderParallelInvocations()
    {
        // Arrange
        var queries = new[]
        {
                new GetEstadoByIdQuery(1),
                new GetEstadoByIdQuery(0),
                new GetEstadoByIdQuery(-1),
                new GetEstadoByIdQuery(5)
            };

        // Act
        var results = queries.AsParallel().Select(q => _validator.TestValidate(q)).ToList();

        // Assert
        Assert.Contains(results, r => r.IsValid); // At least one valid
        Assert.Contains(results, r => !r.IsValid); // At least one invalid
    }
}