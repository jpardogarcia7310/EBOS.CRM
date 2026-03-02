using EBOS.CRM.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.UpdateBranchOfficeAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOfficeAddress.Commands.UpdateBranchOfficeAddress;

public class UpdateBranchOfficeAddressCommandValidatorTest
{
    private readonly UpdateBranchOfficeAddressCommandValidator _validator = new();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateBranchOfficeAddressCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateBranchOfficeAddressRequest BuildUpdateRequest() => new(
            TenantId: 1,
            BranchOfficeId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow,
            ValidTo: null,
            IsCurrent: true
        );
}




