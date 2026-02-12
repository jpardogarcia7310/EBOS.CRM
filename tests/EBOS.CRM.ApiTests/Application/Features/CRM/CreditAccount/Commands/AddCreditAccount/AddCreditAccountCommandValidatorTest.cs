using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;

public class AddCreditAccountCommandValidatorTest
{
    private readonly AddCreditAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddCreditAccountCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddCreditAccountRequest BuildAddRequest() => new(
            TenantId: 1,
            MaxAmount: 1000m,
            UsedAmount: 100m,
            CustomerId: 1
        );
}


