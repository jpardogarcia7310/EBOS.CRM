using EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public class GetIdentificationTypeByIdQueryValidatorTest
{
    private readonly GetIdentificationTypeByIdQueryValidator _validator = new();

    [Fact]
    public async Task Validate_PositiveId_Passes()
    {
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public async Task Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(0);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public async Task Validate_NegativeId_FailsWithCodeAndMessage()
    {
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(-5);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public async Task Validate_MultipleCalls_AreStateless()
    {
        var queryValid = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(10);
        var queryInvalid = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(0);

        var resultValid = await _validator.TestValidateAsync(queryValid);
        var resultInvalid = await _validator.TestValidateAsync(queryInvalid);

        resultValid.ShouldNotHaveValidationErrorFor(q => q.Id);
        resultInvalid.ShouldHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public async Task Validate_ThreadSafety_UnderParallelInvocations()
    {
        var queries = new[]
        {
            new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1),
            new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(0),
            new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(-1),
            new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(5)
        };

        var results = await Task.WhenAll(queries.Select(async query => new
        {
            Query = query,
            Result = await _validator.TestValidateAsync(query)
        }));

        foreach (var item in results)
        {
            if (item.Query.Id > 0)
            {
                item.Result.ShouldNotHaveValidationErrorFor(q => q.Id);
            }
            else
            {
                item.Result.ShouldHaveValidationErrorFor(q => q.Id);
            }
        }
    }
}




