using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;

public class DeleteCreditAccountCommandValidatorTest
{
    private readonly DeleteCreditAccountCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteCreditAccountCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




