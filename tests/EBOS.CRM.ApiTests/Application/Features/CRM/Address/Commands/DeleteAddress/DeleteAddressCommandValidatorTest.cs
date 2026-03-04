using EBOS.CRM.Application.Features.CRM.Address.Commands.DeleteAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Commands.DeleteAddress;

public class DeleteAddressCommandValidatorTest
{
    private readonly DeleteAddressCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidId_Passes()
    {
        var command = new DeleteAddressCommand(1);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteAddressCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




