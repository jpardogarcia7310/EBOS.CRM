using EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.DeleteCreditTransaction;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Commands.DeleteCreditTransaction;

public class DeleteCreditTransactionCommandValidatorTest
{
    private readonly DeleteCreditTransactionCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteCreditTransactionCommand(id);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}