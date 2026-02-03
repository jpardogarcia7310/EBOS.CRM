using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.DeleteBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.DeleteBranchOffice;

public class DeleteBranchOfficeCommandValidatorTest
{
    private readonly DeleteBranchOfficeCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var command = new DeleteBranchOfficeCommand(id);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}


