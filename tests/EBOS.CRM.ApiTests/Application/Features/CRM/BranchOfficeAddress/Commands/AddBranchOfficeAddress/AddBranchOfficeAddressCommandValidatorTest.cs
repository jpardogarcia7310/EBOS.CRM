using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.AddBranchOfficeAddress;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOfficeAddress.Commands.AddBranchOfficeAddress;

public class AddBranchOfficeAddressCommandValidatorTest
{
    private readonly AddBranchOfficeAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddBranchOfficeAddressCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddBranchOfficeAddressRequest BuildAddRequest() => new(
            BranchOfficeId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow,
            ValidTo: null,
            IsCurrent: true
        );
}