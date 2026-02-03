using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public class UpdateIndividualCustomerCommandValidatorTest
{
    private readonly UpdateIndividualCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_InvalidId_Fails()
    {
        var command = new UpdateIndividualCustomerCommand(0, BuildUpdateRequest());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateIndividualCustomerRequest BuildUpdateRequest() => new(
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            CreatedAt: DateTime.UtcNow,
            StatusId: 1,
            FirstName: "John",
            LastName: "Doe",
            BirthDate: DateTime.UtcNow.AddYears(-20),
            IdentificationNumber: "ID123",
            IdentificationTypeId: 1
        );
}


