using EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.AddressesType.Query.GetAddressTypeById;

public class GetAddressTypeByIdValidatorTest
{
    private readonly GetAddressTypeByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_PositiveId_Passes()
    {
        var query = new GetAddressTypeByIdQuery(1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new GetAddressTypeByIdQuery(0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public void Validate_NegativeId_FailsWithCodeAndMessage()
    {
        var query = new GetAddressTypeByIdQuery(-5);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }

    [Fact]
    public void Validate_MultipleCalls_AreStateless()
    {
        var queryValid = new GetAddressTypeByIdQuery(10);
        var queryInvalid = new GetAddressTypeByIdQuery(0);

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
            new GetAddressTypeByIdQuery(1),
            new GetAddressTypeByIdQuery(0),
            new GetAddressTypeByIdQuery(-1),
            new GetAddressTypeByIdQuery(5)
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
