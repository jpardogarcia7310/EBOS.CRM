using EBOS.CRM.Application.Features.CRM.Customer.Services;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Services;

public class CustomerReferenceValidationServiceTest
{
    [Fact]
    public async Task EnsureStatusAndCountryAvailableAsync_WhenStatusMissing_ThrowsValidation()
    {
        var statusRepository = new Mock<IStatusRepository>();
        var countryRepository = new Mock<ICountryRepository>();
        statusRepository.Setup(x => x.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status?)null);

        var service = new CustomerReferenceValidationService(statusRepository.Object, countryRepository.Object);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.EnsureStatusAndCountryAvailableAsync(9, null, CancellationToken.None));
        Assert.Equal("DOMAIN_VALIDATION_CUSTOMER_STATUS_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task EnsureStatusAndCountryAvailableAsync_WhenTimeout_ThrowsTransientDeterministicCode()
    {
        var statusRepository = new Mock<IStatusRepository>();
        var countryRepository = new Mock<ICountryRepository>();
        statusRepository.Setup(x => x.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("status lookup timeout"));

        var service = new CustomerReferenceValidationService(statusRepository.Object, countryRepository.Object);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            service.EnsureStatusAndCountryAvailableAsync(9, null, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_CUSTOMER_REFERENCE_RESOLUTION", ex.Code);
    }
}
