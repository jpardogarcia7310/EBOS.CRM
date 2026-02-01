using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.UpdateCreditTransaction;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Commands.UpdateCreditTransaction;

public class UpdateCreditTransactionCommandValidatorTest
{
    private readonly UpdateCreditTransactionCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCreditTransactionCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

private static UpdateCreditTransactionRequest BuildUpdateRequest() => new(
        Date: DateTime.UtcNow,
        Amount: 50m,
        Type: "Consumption",
        ExternalReference: "REF",
        Comments: "Comment",
        CreditAccountId: 1
    );
}