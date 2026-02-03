using EBOS.CRM.Application.Features.CRM.Address.Commands.DeleteAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Commands.DeleteAddress;

public class DeleteAddressCommandValidatorTest
{
    private readonly DeleteAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidId_Passes()
    {
        var command = new DeleteAddressCommand(1);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteAddressCommand(id);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}


