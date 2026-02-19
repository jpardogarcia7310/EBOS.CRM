using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.AddIndividualCustomer;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Commands.AddIndividualCustomer;

public class AddIndividualCustomerCommandValidatorTest
{
    private readonly AddIndividualCustomerCommandValidator _validator = CreateValidator();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var command = new AddIndividualCustomerCommand(BuildAddRequest());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static AddIndividualCustomerRequest BuildAddRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1,
            FirstName: "John",
            LastName: "Doe",
            BirthDate: DateTime.UtcNow.AddYears(-20),
            IdentificationNumber: "ID123",
            IdentificationTypeId: 1
        );

    private static AddIndividualCustomerCommandValidator CreateValidator()
    {
        var identificationRepo = new Mock<IIdentificationTypeRepository>();
        identificationRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentificationType { Id = 1, Code = "DNI", Description = "DNI", CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = null, UpdatedBy = null, Erased = false });

        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new AddIndividualCustomerCommandValidator(identificationRepo.Object, validationCatalog.Object);
    }
}


