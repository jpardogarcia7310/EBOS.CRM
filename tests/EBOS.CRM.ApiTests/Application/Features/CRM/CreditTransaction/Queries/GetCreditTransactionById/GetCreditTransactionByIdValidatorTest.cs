using EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;

public class GetCreditTransactionByIdQueryValidatorTest
{
    private readonly GetCreditTransactionByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetCreditTransactionByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}