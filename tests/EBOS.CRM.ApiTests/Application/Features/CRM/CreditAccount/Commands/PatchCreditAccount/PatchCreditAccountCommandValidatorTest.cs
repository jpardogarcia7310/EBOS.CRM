using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;
using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;

public class PatchCreditAccountCommandValidatorTest
{
    private readonly PatchCreditAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchCreditAccountRequest(
            TenantId: 1,
            MaxAmount: null,
            UsedAmount: null,
            CustomerId: null);
        var command = new PatchCreditAccountCommand(1, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreditAccountRequest)
            .WithErrorMessage("At least one field must be provided.");
    }
}
