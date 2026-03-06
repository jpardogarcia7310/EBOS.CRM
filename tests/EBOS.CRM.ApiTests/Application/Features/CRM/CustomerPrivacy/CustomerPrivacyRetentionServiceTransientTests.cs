using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy;

public class CustomerPrivacyRetentionServiceTransientTests
{
    [Fact]
    public async Task RunAsync_WhenRepositoryTimeout_ThrowsTransientDomainFailure()
    {
        var privacyRepo = new Mock<ICustomerPrivacyRequestRepository>();
        var tenantConfigRepo = new Mock<ITenantConfigurationRepository>();
        var audit = new Mock<IAuditService>();

        privacyRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("simulated timeout"));

        var sut = new CustomerPrivacyRetentionService(
            privacyRepo.Object,
            tenantConfigRepo.Object,
            audit.Object);

        var act = () => sut.RunAsync(
            tenantId: 1,
            dryRun: true,
            retentionDays: 30,
            batchSize: 100,
            actorUserId: 1,
            correlationId: "corr",
            cancellationToken: CancellationToken.None);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(act);
        Assert.Equal("DOMAIN_TRANSIENT_TIMEOUT", ex.Code);
    }
}
