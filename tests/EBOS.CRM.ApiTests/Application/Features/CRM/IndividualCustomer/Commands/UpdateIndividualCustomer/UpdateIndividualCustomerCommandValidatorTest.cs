using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public class UpdateIndividualCustomerCommandValidatorTest
{
    private readonly UpdateIndividualCustomerCommandValidator _validator = CreateValidator();

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var command = new UpdateIndividualCustomerCommand(0, BuildUpdateRequest());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    private static UpdateIndividualCustomerRequest BuildUpdateRequest() => new(
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

    private static UpdateIndividualCustomerCommandValidator CreateValidator()
    {
        var countryRepo = new Mock<ICountryRepository>();
        countryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Country
            {
                Id = 1,
                Iso31661A2Code = "EC",
                Name = "Ecuador",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                Currency = "USD",
                CurrencyCode = "USD",
                Domain = ".ec",
                InternationalPhoneCode = "593",
                Iso31661A3Code = "ECU",
                Iso31661NumCode = "218"
            });

        var identificationRepo = new Mock<IIdentificationTypeRepository>();
        identificationRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentificationType { Id = 1, Code = "DNI", Description = "DNI", CreatedAt = DateTime.UtcNow, CreatedBy = 1, UpdatedAt = null, UpdatedBy = null, Erased = false });

        var validationCatalog = new Mock<IValidationCatalogService>();
        validationCatalog.Setup(s => s.GetPatternAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        return new UpdateIndividualCustomerCommandValidator(countryRepo.Object, identificationRepo.Object, validationCatalog.Object);
    }
}





