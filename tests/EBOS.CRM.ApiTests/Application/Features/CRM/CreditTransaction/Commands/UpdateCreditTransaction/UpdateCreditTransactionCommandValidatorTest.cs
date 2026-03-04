using EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.UpdateCreditTransaction;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Commands.UpdateCreditTransaction;

public class UpdateCreditTransactionCommandValidatorTest
{
    private readonly UpdateCreditTransactionCommandValidator _validator = new();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateCreditTransactionCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateCreditTransactionRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Date: DateTime.UtcNow,
            Amount: 50m,
            Type: "Consumption",
            ExternalReference: "REF",
            Comments: "Comment",
            CreditAccountId: 1
        );
}




