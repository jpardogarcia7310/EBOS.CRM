using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.DeleteCorporateCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Commands.DeleteCorporateCustomer;

public class DeleteCorporateCustomerCommandValidatorTest
{
    private readonly DeleteCorporateCustomerCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteCorporateCustomerCommand(id);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}


