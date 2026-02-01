using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public class AddBranchOfficeCommandValidatorTest
{
    private readonly AddBranchOfficeCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddBranchOfficeCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddBranchOfficeRequest BuildAddRequest() => new(
            Name: "Main",
            PhoneNumber: "123",
            CorporateCustomerId: 1
        );
}