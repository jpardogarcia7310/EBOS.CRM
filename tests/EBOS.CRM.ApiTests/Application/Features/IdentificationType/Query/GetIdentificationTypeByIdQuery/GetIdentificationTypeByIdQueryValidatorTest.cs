using EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public class GetIdentificationTypeByIdQueryValidatorTest
{
    private readonly GetIdentificationTypeByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_PositiveId_Passes()
    {
        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public void Validate_NegativeId_FailsWithCodeAndMessage()
    {
        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(-5);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public void Validate_MultipleCalls_AreStateless()
    {
        var queryValid = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(10);
        var queryInvalid = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(0);

        var resultValid = _validator.TestValidate(queryValid);
        var resultInvalid = _validator.TestValidate(queryInvalid);

        resultValid.ShouldNotHaveValidationErrorFor(q => q.Id);
        resultInvalid.ShouldHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ThreadSafety_UnderParallelInvocations()
    {
        var queries = new[]
        {
            new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1),
            new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(0),
            new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(-1),
            new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(5)
        };

        Parallel.ForEach(queries, query =>
        {
            var result = _validator.TestValidate(query);
            if (query.Id > 0)
            {
                result.ShouldNotHaveValidationErrorFor(q => q.Id);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(q => q.Id);
            }
        });
    }
}


