using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.DeleteBranchOfficeAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOfficeAddress.Commands.DeleteBranchOfficeAddress;

public class DeleteBranchOfficeAddressCommandValidatorTest
{
    private readonly DeleteBranchOfficeAddressCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteBranchOfficeAddressCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




