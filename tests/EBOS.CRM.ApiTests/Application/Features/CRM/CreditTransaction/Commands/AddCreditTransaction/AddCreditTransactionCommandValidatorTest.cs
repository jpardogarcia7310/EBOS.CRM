using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;

public class AddCreditTransactionCommandValidatorTest
{
    private readonly AddCreditTransactionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddCreditTransactionCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

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


