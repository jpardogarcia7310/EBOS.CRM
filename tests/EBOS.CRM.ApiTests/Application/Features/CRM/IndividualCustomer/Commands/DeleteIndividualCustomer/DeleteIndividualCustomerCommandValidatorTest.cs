using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.DeleteIndividualCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Commands.DeleteIndividualCustomer;

public class DeleteIndividualCustomerCommandValidatorTest
{
    private readonly DeleteIndividualCustomerCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteIndividualCustomerCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




