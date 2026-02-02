using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;

public class UpdateBranchOfficeCommandValidatorTest
{
    private readonly UpdateBranchOfficeCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateBranchOfficeCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateBranchOfficeRequest BuildUpdateRequest() => new(
            Name: "Main",
            PhoneNumber: "123",
            CorporateCustomerId: 1
        );
}