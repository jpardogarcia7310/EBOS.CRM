using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;

public class GetCreditAccountByIdQueryValidatorTest
{
    private readonly GetCreditAccountByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetCreditAccountByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}


