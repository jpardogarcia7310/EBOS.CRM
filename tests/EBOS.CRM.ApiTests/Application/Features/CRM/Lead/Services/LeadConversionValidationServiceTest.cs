using EBOS.CRM.Application.Features.CRM.Lead.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Services;

public class LeadConversionValidationServiceTest
{
    [Fact]
    public async Task EnsureDependenciesAvailableAsync_WhenCustomerMissing_ThrowsValidation()
    {
        var customerRepository = new Mock<ICustomerRepository>();
        var stageRepository = new Mock<IOpportunityStageRepository>();
        customerRepository.Setup(x => x.GetByIdAsync(200, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Customer?)null);

        var service = new LeadConversionValidationService(customerRepository.Object, stageRepository.Object);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.EnsureDependenciesAvailableAsync(1, 200, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_VALIDATION_LEAD_CONVERSION_CUSTOMER_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task EnsureDependenciesAvailableAsync_WhenStageMissing_ThrowsValidation()
    {
        var customerRepository = new Mock<ICustomerRepository>();
        var stageRepository = new Mock<IOpportunityStageRepository>();
        customerRepository.Setup(x => x.GetByIdAsync(200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.Customer { Id = 200, TenantId = 1 });
        stageRepository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage?)null);

        var service = new LeadConversionValidationService(customerRepository.Object, stageRepository.Object);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.EnsureDependenciesAvailableAsync(1, 200, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_VALIDATION_LEAD_CONVERSION_STAGE_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task EnsureDependenciesAvailableAsync_WhenTimeout_ThrowsTransientDeterministicCode()
    {
        var customerRepository = new Mock<ICustomerRepository>();
        var stageRepository = new Mock<IOpportunityStageRepository>();
        customerRepository.Setup(x => x.GetByIdAsync(200, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("customer lookup timeout"));

        var service = new LeadConversionValidationService(customerRepository.Object, stageRepository.Object);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            service.EnsureDependenciesAvailableAsync(1, 200, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_LEAD_CONVERSION_DEPENDENCY_RESOLUTION", ex.Code);
    }
}
