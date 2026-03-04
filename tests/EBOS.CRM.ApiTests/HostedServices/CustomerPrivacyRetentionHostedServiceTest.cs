using EBOS.CRM.Api.HostedServices;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EBOS.CRM.ApiTests.HostedServices;

public class CustomerPrivacyRetentionHostedServiceTest
{
    [Fact]
    public async Task StartAsync_WhenDisabled_DoesNotCreateScope()
    {
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var opts = Microsoft.Extensions.Options.Options.Create(new CustomerPrivacyRetentionJobOptions
        {
            Enabled = false
        });

        var sut = new CustomerPrivacyRetentionHostedService(
            scopeFactory.Object,
            opts,
            new Mock<ILogger<CustomerPrivacyRetentionHostedService>>().Object);

        await sut.StartAsync(CancellationToken.None);
        await sut.StopAsync(CancellationToken.None);

        scopeFactory.Verify(x => x.CreateScope(), Times.Never);
    }

    [Fact]
    public async Task StartAsync_WhenEnabled_ExecutesAtLeastOneSweep()
    {
        var privacyRepo = new Mock<ICustomerPrivacyRequestRepository>();
        var tenantConfigRepo = new Mock<ITenantConfigurationRepository>();
        var auditService = new Mock<IAuditService>();

        var req = CustomerPrivacyRequest.Create(1, 10, CustomerPrivacyRequest.TypeAnonymize, 1, null, null);
        req.MarkInProgress(1);
        req.MarkCompleted(1);

        privacyRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CustomerPrivacyRequest> { req });
        tenantConfigRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TenantConfiguration>());
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var retention = new CustomerPrivacyRetentionService(
            privacyRepo.Object,
            tenantConfigRepo.Object,
            auditService.Object);

        var services = new ServiceCollection();
        services.AddSingleton(privacyRepo.Object);
        services.AddSingleton(retention);
        var provider = services.BuildServiceProvider();

        var opts = Microsoft.Extensions.Options.Options.Create(new CustomerPrivacyRetentionJobOptions
        {
            Enabled = true,
            DryRun = true,
            SweepIntervalMinutes = 1,
            BatchSize = 100,
            SystemUserId = 1
        });

        var sut = new CustomerPrivacyRetentionHostedService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            opts,
            new Mock<ILogger<CustomerPrivacyRetentionHostedService>>().Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));
        await sut.StartAsync(cts.Token);
        await Task.Delay(120, CancellationToken.None);
        await sut.StopAsync(CancellationToken.None);

        privacyRepo.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
