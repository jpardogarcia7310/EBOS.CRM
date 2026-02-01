using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;

public class UpdateCorporateCustomerCommandValidatorTest
{
    private readonly UpdateCorporateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateCorporateCustomerCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

private static UpdateCorporateCustomerRequest BuildUpdateRequest() => new(
        Code: "C001",
        Email: "a@b.com",
        Phone: "123",
        CreatedAt: DateTime.UtcNow,
        StatusId: 1,
        LegalName: "Corp",
        TaxIdentification: "TAX999"
    );
}