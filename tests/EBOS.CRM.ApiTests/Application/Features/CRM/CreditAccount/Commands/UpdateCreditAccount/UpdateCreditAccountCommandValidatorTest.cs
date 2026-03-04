using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;

public class UpdateCreditAccountCommandValidatorTest
{
    private readonly UpdateCreditAccountCommandValidator _validator = new();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateCreditAccountCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateCreditAccountRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Id: 1,
            MaxAmount: 1000m,
            UsedAmount: 100m,
            CustomerId: 1
        );
}




