using EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.TenantQuota.Queries.GetTenantQuotaById;

public class GetTenantQuotaByIdQueryValidatorTest
{
    private readonly GetTenantQuotaByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_PositiveId_Passes()
    {
        var query = new GetTenantQuotaByIdQuery(1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveValidationErrorFor(q => q.Id);
    }

    [Fact]
    public void Validate_ZeroId_FailsWithCodeAndMessage()
    {
        var query = new GetTenantQuotaByIdQuery(0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Id)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithErrorMessage("The identifier must be a positive integer greater than 0.");
    }
}
