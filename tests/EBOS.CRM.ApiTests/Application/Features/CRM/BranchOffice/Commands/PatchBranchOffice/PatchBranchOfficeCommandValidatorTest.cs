using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public class PatchBranchOfficeCommandValidatorTest
{
    private readonly PatchBranchOfficeCommandValidator _validator = new();

    [Fact]
    public void Validate_NoPatchFields_ReturnsError()
    {
        var request = new PatchBranchOfficeRequest(
            TenantId: 1,
            Name: null,
            PhoneNumber: null,
            CorporateCustomerId: null);
        var command = new PatchBranchOfficeCommand(1, request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BranchOfficeRequest)
            .WithErrorMessage("At least one field must be provided.");
    }
}
