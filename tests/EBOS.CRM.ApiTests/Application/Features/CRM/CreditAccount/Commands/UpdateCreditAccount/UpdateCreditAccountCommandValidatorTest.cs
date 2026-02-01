using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;

public class UpdateCreditAccountCommandValidatorTest
{
    private readonly UpdateCreditAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCreditAccountCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateCreditAccountRequest BuildUpdateRequest() => new(
            MaxAmount: 1000m,
            UsedAmount: 100m,
            CustomerId: 1
        );
}