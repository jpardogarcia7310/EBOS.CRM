using EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;

public class AddCreditTransactionCommandValidatorTest
{
    private readonly AddCreditTransactionCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new AddCreditTransactionCommand(BuildAddRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddCreditTransactionRequest BuildAddRequest() => new(
            TenantId: 1,
            Date: DateTime.UtcNow,
            Amount: 50m,
            Type: "Consumption",
            ExternalReference: "REF",
            Comments: "Comment",
            CreditAccountId: 1
        );
}




