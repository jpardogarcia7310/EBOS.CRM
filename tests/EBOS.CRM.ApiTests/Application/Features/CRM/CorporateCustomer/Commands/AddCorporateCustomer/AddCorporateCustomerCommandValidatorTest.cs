using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;

public class AddCorporateCustomerCommandValidatorTest
{
    private readonly AddCorporateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddCorporateCustomerCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddCorporateCustomerRequest BuildAddRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1,
            LegalName: "Corp",
            TaxIdentification: "TAX999"
        );
}


